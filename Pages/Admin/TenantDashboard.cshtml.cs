using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Data;

namespace Pages.Admin
{
    public class TenantDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Tenant? Tenant { get; set; }
        public TenantBranding? Branding { get; set; }
        public TenantSettings? Settings { get; set; }
        public TenantModules? Modules { get; set; }

        public TenantDashboardModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(Guid? tenantId)
        {
            if (tenantId == null)
                return NotFound();

            Tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (Tenant == null)
                return NotFound();

            Branding = await _db.TenantBrandings.FirstOrDefaultAsync(b => b.TenantId == tenantId);
            Settings = await _db.TenantSettings.FirstOrDefaultAsync(s => s.TenantId == tenantId);
            Modules = await _db.TenantModules.FirstOrDefaultAsync(m => m.TenantId == tenantId);

            return Page();
        }
    }
}
