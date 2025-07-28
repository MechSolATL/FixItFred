using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class TechTrackingModel : PageModel
    {
        // Sprint 91.7: Inject PermissionService only (DeviceDetectionService removed)
        private readonly PermissionService _permissionService;
        private readonly ApplicationDbContext _db;

        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();
        public List<MVP_Core.Data.TechTrackingLog> TechTrackingLogs { get; set; } = new();

        public TechTrackingModel(PermissionService permissionService, ApplicationDbContext db)
        {
            _permissionService = permissionService;
            _db = db;
        }

        public void OnGet()
        {
            // Sprint 91.7: Admin-only access enforced by attribute
            Technicians = _db.Technicians.Where(t => t.IsActive).ToList();
            // Optionally: Preload last 5 tracking logs per tech for ghost trails
        }
    }
}
