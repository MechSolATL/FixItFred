using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Tenant
{
    public class TenantResolver
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession? _session;
        public Guid? TenantId { get; private set; }
        public TenantBranding? TenantBranding { get; private set; }
        public TenantModules? TenantModules { get; private set; }
        public bool IsResolved { get; private set; }

        public TenantResolver(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext?.Session;
            ResolveTenant();
        }

        private void ResolveTenant()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return;

            // 1. Querystring
            string? tenantIdStr = context.Request.Query["tenantId"].ToString();
            Guid? tenantId = null;
            if (Guid.TryParse(tenantIdStr, out var parsedId))
            {
                tenantId = parsedId;
                _session?.SetString("TenantId", tenantId.ToString());
            }
            // 2. Session
            else if (_session?.GetString("TenantId") is string sessionId && Guid.TryParse(sessionId, out var sessionGuid))
            {
                tenantId = sessionGuid;
            }
            // 3. Fallback: first active tenant
            else
            {
                tenantId = _db.Tenants.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.CreatedAt).Select(t => t.Id).FirstOrDefault();
                if (tenantId != Guid.Empty)
                    _session?.SetString("TenantId", tenantId.ToString());
            }

            if (tenantId == null || tenantId == Guid.Empty)
            {
                IsResolved = false;
                return;
            }

            TenantId = tenantId;
            TenantBranding = _db.TenantBrandings.AsNoTracking().FirstOrDefault(b => b.TenantId == tenantId);
            TenantModules = _db.TenantModules.AsNoTracking().FirstOrDefault(m => m.TenantId == tenantId);
            IsResolved = true;
        }
    }
}
