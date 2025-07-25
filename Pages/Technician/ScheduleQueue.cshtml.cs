using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MVP_Core.Pages.Technician
{
    // Sprint 31.1 - Technician Schedule Acceptance Workflow
    [Authorize(Roles = "Technician")]
    public class ScheduleQueueModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public int TechnicianId { get; set; }
        public List<ScheduleQueue> PendingJobs { get; set; } = new();
        public ScheduleQueueModel(ApplicationDbContext db) { _db = db; }
        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null && int.TryParse(idClaim.Value, out int tid))
                {
                    TechnicianId = tid;
                    PendingJobs = _db.ScheduleQueues
                        .Where(q => q.TechnicianId == tid && q.Status == ScheduleStatus.Pending)
                        .OrderBy(q => q.EstimatedArrival)
                        .ToList();
                }
            }
        }
    }
}
