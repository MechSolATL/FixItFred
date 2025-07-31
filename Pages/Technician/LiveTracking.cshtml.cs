using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Pages.Technician
{
    // Sprint 30E.5 - Technician Mobile Tracking View UI
    [Authorize(Roles = "Technician")]
    public class LiveTrackingModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public int TechnicianId { get; set; }
        public List<ScheduleQueue> ActiveJobs { get; set; } = new();
        public LiveTrackingModel(ApplicationDbContext db) { _db = db; }
        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null && int.TryParse(idClaim.Value, out int tid))
                {
                    TechnicianId = tid;
                    ActiveJobs = _db.ScheduleQueues
                        .Where(q => q.TechnicianId == tid && q.Status == ScheduleStatus.Pending)
                        .OrderBy(q => q.EstimatedArrival)
                        .ToList();
                }
            }
        }
    }
}
