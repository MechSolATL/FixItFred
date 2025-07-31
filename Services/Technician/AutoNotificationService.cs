using System;
using System.Threading.Tasks;
using Data;
using MVP_Core.Models;

namespace Services.Technician
{
    // Sprint 86.7 — Auto Notification Service for Technician AI Companion
    public class AutoNotificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly NotificationService _notificationService;
        public AutoNotificationService(ApplicationDbContext db, NotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }
        public async Task TriggerLateETAAlert(int techId, int jobId, DateTime estimatedArrival)
        {
            var tech = await _db.Technicians.FindAsync(techId);
            var job = await _db.ServiceRequests.FindAsync(jobId);
            if (tech == null || job == null) return;
            var now = DateTime.UtcNow;
            if (now > estimatedArrival)
            {
                // Send alert to customer and admin
                await _notificationService.SendAsync(job.CustomerId, $"Technician {tech.FullName} is running late for your job #{job.Id}.");
                await _notificationService.SendAsync("admin@company.com", $"Late ETA: Tech {tech.FullName} for job #{job.Id}.");
                // Log alert
                _db.TechnicianAlertLogs.Add(new TechnicianAlertLog
                {
                    TechnicianId = techId,
                    ServiceRequestId = jobId,
                    AlertType = "LateETA",
                    CreatedAt = now,
                    Message = $"Late ETA detected for job #{job.Id}"
                });
                await _db.SaveChangesAsync();
            }
        }
    }
}
