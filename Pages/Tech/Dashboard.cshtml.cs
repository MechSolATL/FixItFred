// Sprint 91.7 Part 7: Technician Dashboard View + Embedded Route Guidance + Action Card Stack
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Data.DTO.Tools;
using Data;
using Data.Models;

namespace Pages.Tech
{
    [Authorize(Roles = "Technician")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DashboardModel(ApplicationDbContext db)
        {
            _db = db;
        }
        // Route/Job Data
        public ScheduleQueue? CurrentJob { get; set; }
        public ScheduleQueue? NextJob { get; set; }
        public string CurrentJobStatus { get; set; } = "En Route";
        public string? CurrentJobClientName { get; set; }
        public List<ToolDto> ToolsRequired { get; set; } = new();
        public List<string> Alerts { get; set; } = new();
        public bool NextJobOverlapWarning { get; set; }
        // Map/Location (mocked for now)
        public double CurrentLat { get; set; } = 33.7490; // Atlanta default
        public double CurrentLng { get; set; } = -84.3880;
        public double? NextJobLat { get; set; } = null;
        public double? NextJobLng { get; set; } = null;
        public double MockDistanceKm { get; set; } = 7.2;
        public async Task<IActionResult> OnGetAsync()
        {
            int techId = 0;
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(idClaim, out techId);
            }
            if (techId == 0)
                return Unauthorized();
            // Get jobs (ordered by ETA)
            var jobs = _db.ScheduleQueues.Where(q => q.TechnicianId == techId && q.Status == ScheduleStatus.Pending)
                .OrderBy(q => q.EstimatedArrival).ToList();
            CurrentJob = jobs.FirstOrDefault();
            NextJob = jobs.Skip(1).FirstOrDefault();
            // Mock: Get client name (replace with join if needed)
            CurrentJobClientName = "John Q. Customer";
            // Mock: Set next job lat/lng
            if (NextJob != null)
            {
                NextJobLat = 33.7550;
                NextJobLng = -84.3900;
                // Overlap warning if jobs overlap by < 30 min
                if (CurrentJob?.EstimatedArrival != null && NextJob.ScheduledFor != null)
                {
                    var diff = (NextJob.ScheduledFor.Value - CurrentJob.EstimatedArrival.Value).TotalMinutes;
                    NextJobOverlapWarning = diff < 30;
                }
            }
            // Tools required (mocked)
            ToolsRequired = new List<ToolDto> {
                new ToolDto { Name = "Cordless Drill", ToolType = "Power Tool" },
                new ToolDto { Name = "Pipe Wrench", ToolType = "Hand Tool" }
            };
            // Alerts (mocked)
            Alerts = new List<string>();
            if (CurrentJob?.IsEscalated == true)
                Alerts.Add("SLA Escalation: Immediate action required!");
            if (NextJobOverlapWarning)
                Alerts.Add("Next job may overlap with current job.");
            // Status badge (mocked)
            CurrentJobStatus = "En Route";
            return Page();
        }
        // Color code for urgency
        public string GetUrgencyClass(ScheduleQueue? job)
        {
            if (job == null) return "";
            if (job.IsEmergency) return "red";
            if (job.IsUrgent) return "yellow";
            return "green";
        }
    }
}
