// Sprint 44 – Message Export + Digest
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using Data;
using Services;

namespace Controllers.Api
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsApiController : ControllerBase
    {
        private readonly NotificationDigestService _digestService;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _db;
        public NotificationsApiController(NotificationDigestService digestService, INotificationService notificationService, ApplicationDbContext db)
        {
            _digestService = digestService;
            _notificationService = notificationService;
            _db = db;
        }
        [HttpPost("trigger-digest")]
        public async Task<IActionResult> TriggerDigest([FromQuery] string? toEmail = null)
        {
            // For admin testing: do not send real email unless toEmail is provided
            var html = await _digestService.TriggerDigestAsync(!string.IsNullOrEmpty(toEmail), _notificationService, toEmail ?? "admin@example.com");
            return Content(html, "text/html");
        }

        // Sprint 45 – Device Registration Endpoint
        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistrationDto dto)
        {
            if (dto.TechnicianId <= 0 || string.IsNullOrWhiteSpace(dto.DeviceToken) || string.IsNullOrWhiteSpace(dto.Platform))
                return BadRequest("Missing required fields.");
            var existing = _db.TechnicianDeviceRegistrations.FirstOrDefault(r => r.TechnicianId == dto.TechnicianId && r.DeviceToken == dto.DeviceToken);
            if (existing != null)
            {
                existing.LastSeen = DateTime.UtcNow;
                existing.Platform = dto.Platform;
            }
            else
            {
                _db.TechnicianDeviceRegistrations.Add(new TechnicianDeviceRegistration
                {
                    TechnicianId = dto.TechnicianId,
                    DeviceToken = dto.DeviceToken,
                    Platform = dto.Platform,
                    LastSeen = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }
        public class DeviceRegistrationDto
        {
            public int TechnicianId { get; set; }
            public string DeviceToken { get; set; } = string.Empty;
            public string Platform { get; set; } = string.Empty;
        }
    }
}
