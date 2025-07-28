// FixItFred Patch Log — CS1998 Async Compliance Patch
// Sprint83.4-IsOnlinePatch: Fixed CS0200 read-only property assignment
// Top-level statement for Technicians initialization removed. Initialization is handled inside OnGetAsync.
// 2024-07-24T21:16:00Z
// Applied Fixes: CS1998
// Notes: Inserted await Task.CompletedTask in async methods without awaits for compliance.
// FixItFred Patch Log — Sprint 26.2D
// [2025-07-25T00:00:00Z] — Final async Razor binding for TechnicianDropdownViewModel. CS0118/CS1061 resolved.
// FixItFred Patch Log — Sprint 28 Recovery Patch
// [2024-07-25T00:40:00Z] — Added ServiceZones property for zone filtering in dispatcher UI.
// FixItFred Patch Log — Sprint 28
// [2025-07-25T00:00:00Z] — ServiceZones property exposed and Razor reference corrected for Dispatcher view.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models;
using MVP_Core.Data.Models;
using MVP_Core.Services.Dispatcher;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class DispatcherModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly TechMapService _mapService;
        public DispatcherViewModel ViewModel { get; set; } = new();
        public PermissionService PermissionService { get; }
        public AdminUser AdminUser { get; }
        public DispatcherModel(ApplicationDbContext db, PermissionService permissionService)
        {
            _db = db;
            _mapService = new TechMapService(db);
            PermissionService = permissionService;
            // For demo: get current admin user from context/session/service
            AdminUser = HttpContext?.Items["AdminUser"] as AdminUser ?? new AdminUser { EnabledModules = new List<string>() };
        }
        public void OnGet()
        {
            ViewModel.Technicians = _db.Technicians.Where(t => t.IsActive).ToList();
            ViewModel.ServiceRequests = _db.ServiceRequests.Where(r => r.AssignedTechId == null).ToList();
            ViewModel.TechMapPins = _mapService.GetActiveTechnicianPins();
        }
        // Assignment logic would go here (AJAX endpoint or OnPost handler)
    }
}
