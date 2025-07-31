using Data;
using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data.Models;
using MVP_Core.Models.Admin;
using Services.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Controllers.Api
{
    [ApiController]
    [Route("api/scheduler")]
    public class SchedulerApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly DispatcherService _dispatcherService;

        public SchedulerApiController(ApplicationDbContext db, DispatcherService dispatcherService)
        {
            _db = db;
            _dispatcherService = dispatcherService;
        }

        public class UpdateJobTimeDto
        {
            public int JobId { get; set; }
            public int? TechnicianId { get; set; }
            public DateTime? NewEta { get; set; }
            public DateTime? NewSla { get; set; } // Optional, ignored for ServiceRequest
            public string? Day { get; set; } // Optional, for calendar UI
        }

        [HttpPost("update-job-time")]
        public async Task<IActionResult> UpdateJobTime([FromBody] UpdateJobTimeDto dto)
        {
            if (dto.JobId <= 0 || !dto.NewEta.HasValue)
                return BadRequest(new { success = false, error = "Missing job ID or ETA." });

            var job = _db.ServiceRequests.FirstOrDefault(r => r.Id == dto.JobId);
            if (job == null)
                return NotFound(new { success = false, error = "Job not found." });

            // Update ETA
            job.DueDate = dto.NewEta.Value;

            // Update technician assignment if provided
            if (dto.TechnicianId.HasValue)
            {
                var tech = _db.Technicians.FirstOrDefault(t => t.Id == dto.TechnicianId.Value && t.IsActive);
                if (tech == null)
                    return BadRequest(new { success = false, error = "Technician not found or inactive." });
                job.AssignedTechnicianId = tech.Id;
                job.AssignedTo = tech.FullName;
            }

            // SLAExpiresAt is not present on ServiceRequest, so skip
            // Optionally update day (for calendar UI, if needed)
            // (Assume day is handled via ETA or other logic)

            await _db.SaveChangesAsync();

            // Optionally: SignalR or notification logic here

            return Ok(new { success = true, jobId = job.Id, eta = job.DueDate, technicianId = job.AssignedTechnicianId });
        }
    }
}
