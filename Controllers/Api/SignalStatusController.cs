using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using Data;

namespace Controllers.Api
{
    [ApiController]
    [Route("api/signal")]
    public class SignalStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public SignalStatusController(ApplicationDbContext db)
        {
            _db = db;
        }

        public class SignalStatusDto
        {
            public int TechnicianId { get; set; }
            public string EventType { get; set; } = string.Empty; // offline_start, offline_end
            public DateTime Timestamp { get; set; }
            public LocationDto? Location { get; set; }
        }
        public class LocationDto
        {
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
        }

        [HttpPost("status")]
        public async Task<IActionResult> PostStatus([FromBody] SignalStatusDto dto)
        {
            if (dto.TechnicianId <= 0) return BadRequest("TechnicianId required");
            var now = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp;
            var session = _db.TechnicianOfflineSessions
                .Where(s => s.TechnicianId == dto.TechnicianId && s.EndTime == null)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefault();
            if (dto.EventType == "offline_start")
            {
                if (session == null)
                {
                    _db.TechnicianOfflineSessions.Add(new TechnicianOfflineSession
                    {
                        TechnicianId = dto.TechnicianId,
                        StartTime = now,
                        LocationZip = GetZipFromLocation(dto.Location),
                        Notes = "Offline session started"
                    });
                    await _db.SaveChangesAsync();
                }
            }
            else if (dto.EventType == "offline_end")
            {
                if (session != null)
                {
                    session.EndTime = now;
                    session.Notes = (session.Notes ?? "") + "; Offline session ended";
                    await _db.SaveChangesAsync();
                }
            }
            return Ok(new { success = true });
        }

        private string? GetZipFromLocation(LocationDto? loc)
        {
            // TODO: Implement reverse geocoding if needed
            return null;
        }
    }
}
