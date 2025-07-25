// Sprint 32.2 - Security + Audit Harden
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    public interface IAuditTrailLogger
    {
        Task LogAsync(string userId, string action, string ip, string? additionalData = null);
    }

    public class AuditTrailLogger : IAuditTrailLogger
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        public AuditTrailLogger(ApplicationDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }
        public async Task LogAsync(string userId, string action, string ip, string? additionalData = null)
        {
            _db.AuditLogEntries.Add(new AuditLogEntry
            {
                UserId = userId,
                Action = action,
                IPAddress = ip,
                Timestamp = DateTime.UtcNow,
                AdditionalData = additionalData
            });
            await _db.SaveChangesAsync();
        }
    }
}
