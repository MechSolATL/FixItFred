using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using MVP_Core.Services.Dispatch;
using System.Threading.Tasks;
using MVP_Core.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;

namespace MVP_Core.Controllers.Api
{
    [ApiController]
    public class TechnicianApiController : ControllerBase
    {
        private readonly IAuditTrailLogger _auditLogger;
        public TechnicianApiController(IAuditTrailLogger auditLogger) { _auditLogger = auditLogger; }

        [HttpGet("/api/technicians/active")]
        public async Task<IActionResult> GetActiveTechnicians([FromServices] ITechnicianService techService)
        {
            if (techService == null) throw new ArgumentNullException(nameof(techService)); // CS8604 fix
            var techs = await techService.GetAllAsync(); // Returns List<TechnicianViewModel>
            var activeTechs = techs?.Where(t => t.IsActive).ToList() ?? new List<TechnicianViewModel>();
            return Ok(activeTechs ?? new List<TechnicianViewModel>()); // Sprint 79.7: TechnicianApiController cleanup
        }

        [Authorize(Roles = "Technician")]
        [HttpPost("/api/tech/update-location")]
        public async Task<IActionResult> UpdateLocation(
            [FromServices] ApplicationDbContext db,
            [FromServices] DispatcherService dispatcherService,
            [FromServices] NotificationDispatchEngine dispatchEngine,
            [FromBody] TechnicianLocationUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int techId))
                return Unauthorized();
            if (dto == null || dto.TechnicianId != techId) // Sprint 79.7: TechnicianApiController cleanup
                return Forbid();

            var lastLog = db.TechTrackingLogs
                .Where(l => l.TechnicianId == techId)
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();
            if (lastLog != null && (DateTime.UtcNow - lastLog.Timestamp).TotalSeconds < 15)
                return StatusCode(429, new { error = "Too many updates. Please wait before sending again." });

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();
            db.TechTrackingLogs.Add(new MVP_Core.Data.TechTrackingLog {
                TechnicianId = techId,
                Timestamp = DateTime.UtcNow,
                IP = ip,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                UserAgent = userAgent
            });
            await db.SaveChangesAsync();

            var tech = await db.Technicians.FindAsync(techId);
            if (tech == null) return NotFound();
            tech.Latitude = dto.Latitude;
            tech.Longitude = dto.Longitude;
            await db.SaveChangesAsync();

            var queues = db.ScheduleQueues.Where(q => q.TechnicianId == tech.Id && q.Status == ScheduleStatus.Pending).ToList();
            foreach (var queue in queues)
            {
                var techProfile = db.Set<MVP_Core.Data.Models.TechnicianProfileDto>().FirstOrDefault(t => t.Id == tech.Id);
                if (techProfile != null)
                {
                    techProfile.Latitude = dto.Latitude;
                    techProfile.Longitude = dto.Longitude;
                    var techStatus = new MVP_Core.Models.Admin.TechnicianStatusDto {
                        TechnicianId = techProfile.Id,
                        Name = techProfile.FullName ?? string.Empty, // Sprint 79.7: TechnicianApiController cleanup
                        Status = techProfile.Specialty ?? string.Empty, // Sprint 79.7: TechnicianApiController cleanup
                        DispatchScore = 100,
                        LastPing = DateTime.UtcNow,
                        AssignedJobs = 0,
                        LastUpdate = DateTime.UtcNow
                    };
                    var eta = dispatcherService.PredictETA(techStatus, queue.Zone ?? string.Empty, 0).GetAwaiter().GetResult(); // Sprint 79.7: TechnicianApiController cleanup
                    queue.EstimatedArrival = eta;
                    await db.SaveChangesAsync();
                    await dispatchEngine.BroadcastETAAsync(queue.Zone ?? string.Empty, $"Technician {tech.FullName ?? "Unknown"} ETA: {eta:t}"); // Sprint 79.7: TechnicianApiController cleanup
                }
            }
            await _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "TechLocationUpdate", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"Lat={dto.Latitude};Lng={dto.Longitude}");
            return Ok(new { success = true });
        }

        [HttpGet("/api/tech/active-jobs")]
        public IActionResult GetActiveJobs([FromServices] ApplicationDbContext db, int techId)
        {
            var jobs = db.ScheduleQueues
                .Where(q => q.TechnicianId == techId && q.Status == ScheduleStatus.Pending)
                .Select(q => new {
                    id = q.Id,
                    serviceRequestId = q.ServiceRequestId,
                    zone = q.Zone ?? string.Empty, // Sprint 79.7: TechnicianApiController cleanup
                    estimatedArrival = q.EstimatedArrival,
                    status = q.Status.ToString()
                }).ToList();
            return Ok(jobs ?? new List<object>()); // Sprint 79.7: TechnicianApiController cleanup
        }

        [HttpPost("/api/tech/respond-to-schedule")]
        public async Task<IActionResult> RespondToSchedule(
            [FromServices] ApplicationDbContext db,
            [FromServices] IHubContext<ETAHub> hubContext,
            [FromBody] ScheduleResponseDto dto)
        {
            if (dto == null) return BadRequest(); // Sprint 79.7: TechnicianApiController cleanup
            var entry = await db.ScheduleQueues.FindAsync(dto.ScheduleQueueId);
            if (entry == null) return NotFound();
            if (dto.Response == "Accepted")
                entry.Status = ScheduleStatus.Dispatched;
            else if (dto.Response == "Declined")
                entry.Status = ScheduleStatus.Cancelled;
            else
                return BadRequest();
            await db.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ScheduleQueueUpdated", entry.Id, entry.Status.ToString());
            await _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "TechScheduleResponse", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"ScheduleQueueId={dto.ScheduleQueueId};Response={dto.Response}");
            return Ok(new { success = true });
        }
        public class ScheduleResponseDto
        {
            public int ScheduleQueueId { get; set; }
            public string Response { get; set; } = string.Empty;
        }
    }

    public class TechnicianLocationUpdateDto
    {
        public int TechnicianId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
