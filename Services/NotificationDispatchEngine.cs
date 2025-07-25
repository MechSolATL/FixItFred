// FixItFred Patch Log — Sprint 26.4
// [2025-07-25T00:00:00Z] — NotificationDispatchEngine service scaffolded for scheduler queue and notification logic.
using MVP_Core.Models.Dispatch;
using MVP_Core.Models.Admin;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    public class NotificationDispatchEngine
    {
        // Simulated in-memory queue for demonstration
        private static List<ScheduleQueue> _queue = new();
        private static List<ScheduleHistory> _history = new();
        private static List<NotificationsSent> _notifications = new();
        private readonly DispatcherService _dispatcherService;

        public NotificationDispatchEngine(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public IReadOnlyList<ScheduleQueue> GetQueue(string? regionId = null)
        {
            if (string.IsNullOrEmpty(regionId)) return _queue;
            return _queue.Where(q => q.RegionId == regionId).ToList();
        }
        public IReadOnlyList<ScheduleHistory> GetHistory(string? regionId = null)
        {
            if (string.IsNullOrEmpty(regionId)) return _history;
            return _history.Where(h => h.RegionId == regionId).ToList();
        }
        public IReadOnlyList<NotificationsSent> GetNotifications(string? regionId = null)
        {
            if (string.IsNullOrEmpty(regionId)) return _notifications;
            return _notifications.Where(n => n.RegionId == regionId).ToList();
        }
        public void Enqueue(int technicianId, int serviceRequestId, DateTime scheduledFor, string? regionId = null)
        {
            var entry = new ScheduleQueue
            {
                TechnicianId = technicianId,
                ServiceRequestId = serviceRequestId,
                ScheduledFor = scheduledFor,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                RegionId = regionId
            };
            _queue.Add(entry);
            _history.Add(new ScheduleHistory
            {
                ScheduleQueueId = entry.Id,
                Action = "Enqueued",
                Timestamp = DateTime.UtcNow,
                Notes = $"Technician {technicianId} scheduled for request {serviceRequestId}",
                RegionId = regionId
            });
        }
        public void MarkAsNotified(int technicianId, int serviceRequestId, string notificationType, string message, string? regionId = null)
        {
            _notifications.Add(new NotificationsSent
            {
                TechnicianId = technicianId,
                ServiceRequestId = serviceRequestId,
                NotificationType = notificationType,
                SentAt = DateTime.UtcNow,
                Message = message,
                RegionId = regionId
            });
            var entry = _queue.FirstOrDefault(q => q.TechnicianId == technicianId && q.ServiceRequestId == serviceRequestId && q.RegionId == regionId);
            if (entry != null)
            {
                entry.Status = "Notified";
                _history.Add(new ScheduleHistory
                {
                    ScheduleQueueId = entry.Id,
                    Action = "Notified",
                    Timestamp = DateTime.UtcNow,
                    Notes = message,
                    RegionId = regionId
                });
            }
        }

        // Example: Link to technician availability and ETA prediction
        public bool IsTechnicianAvailable(int technicianId, DateTime forTime)
        {
            // TODO: Integrate with TechnicianStatusDto and real availability logic
            return !_queue.Any(q => q.TechnicianId == technicianId && q.ScheduledFor == forTime && q.Status != "Complete");
        }

        public TimeSpan PredictETA(int technicianId, int serviceRequestId)
        {
            // TODO: Integrate with LoadBalancingService or route logic
            return TimeSpan.FromMinutes(30); // Stub: always 30 min
        }

        // Sprint 26.4B: Annotate queue with live technician status and ETA
        public async Task AnnotateScheduleQueueAsync(List<ScheduleQueue> queue)
        {
            var technicians = await _dispatcherService.GetTechniciansAsync();
            foreach (var request in queue)
            {
                var bestTech = technicians
                    .Where(t => t.SkillTags != null && t.SkillTags.Any() && t.TopZIPs != null && t.TopZIPs.Contains(request.ZipCode ?? ""))
                    .OrderBy(t => EstimateTravelTime(t.TopZIPs.FirstOrDefault() ?? "", request.ZipCode ?? ""))
                    .FirstOrDefault();
                if (bestTech != null)
                {
                    request.AssignedTechnicianName = bestTech.Name;
                    request.TechnicianStatus = "Available"; // Stub: real status lookup can be added
                    request.EstimatedArrival = DateTime.UtcNow.AddMinutes(EstimateTravelTime(bestTech.TopZIPs.FirstOrDefault() ?? "", request.ZipCode ?? ""));
                }
                else
                {
                    request.AssignedTechnicianName = null;
                    request.TechnicianStatus = "Unavailable";
                    request.EstimatedArrival = null;
                }
            }
        }
        private int EstimateTravelTime(string fromZip, string toZip)
        {
            // Placeholder: real version should use Google Maps API or routing logic
            return fromZip == toZip ? 10 : 25;
        }

        // FixItFred Patch Log — Sprint 26.4E
        // [2025-07-25T00:00:00Z] — GetETAHistory method updated to use shared ETAHistoryEntry model. Fixed misplaced using and braces.
        public List<ETAHistoryEntry> GetETAHistory()
        {
            // Stub: Simulated ETA history for demonstration
            return new List<ETAHistoryEntry>
            {
                new ETAHistoryEntry {
                    TechnicianName = "Alice Smith",
                    ServiceRequestId = 101,
                    PredictedETA = DateTime.UtcNow.AddMinutes(-40),
                    ActualArrival = DateTime.UtcNow.AddMinutes(-35),
                    Timestamp = DateTime.UtcNow.AddMinutes(-40)
                },
                new ETAHistoryEntry {
                    TechnicianName = "Bob Jones",
                    ServiceRequestId = 102,
                    PredictedETA = DateTime.UtcNow.AddMinutes(-30),
                    ActualArrival = DateTime.UtcNow.AddMinutes(-28),
                    Timestamp = DateTime.UtcNow.AddMinutes(-30)
                },
                new ETAHistoryEntry {
                    TechnicianName = "Carlos Lee",
                    ServiceRequestId = 103,
                    PredictedETA = DateTime.UtcNow.AddMinutes(-20),
                    ActualArrival = null,
                    Timestamp = DateTime.UtcNow.AddMinutes(-20)
                }
            };
        }
    }
}
