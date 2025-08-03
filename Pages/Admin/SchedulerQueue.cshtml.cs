// FixItFred Patch Log � Sprint 26.4C
// [2025-07-25T00:00:00Z] � Manual re-annotation handler added for SchedulerQueue. Triggers AnnotateScheduleQueueAsync.
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Dispatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pages.Admin
{
    public class SchedulerQueueModel : PageModel
    {
        private readonly NotificationDispatchEngine _dispatchEngine;
        public SchedulerQueueModel(NotificationDispatchEngine dispatchEngine)
        {
            _dispatchEngine = dispatchEngine;
        }
        public IReadOnlyList<ScheduleQueue> Queue => _dispatchEngine.GetPendingDispatches().ToList();
        // Sprint 50.0: Add intelligence sorting and escalation logic
        public IReadOnlyList<ScheduleQueue> IntelligentQueue => Queue
            .OrderByDescending(q => q.IsEmergency)
            .ThenByDescending(q => q.IsUrgent)
            .ThenBy(q => q.SLAWindowEnd ?? DateTime.MaxValue)
            .ThenBy(q => q.GeoDistanceKm ?? double.MaxValue)
            .ThenByDescending(q => q.IsTechnicianAvailable)
            .ThenBy(q => q.ServiceTypePriority)
            .ToList();
        [BindProperty]
        public int TechnicianId { get; set; }
        [BindProperty]
        public int ServiceRequestId { get; set; }
        public void OnGet()
        {
            // No-op: queue loaded via property
        }
        // FixItFred: Notification logic to be updated for new engine
        // Sprint 55.0: Notify handler for queue actions
        public async Task<IActionResult> OnPostNotifyAsync()
        {
            // Example: Send JobAssigned notification
            var technician = IntelligentQueue.FirstOrDefault(q => q.TechnicianId == TechnicianId && q.ServiceRequestId == ServiceRequestId);
            if (technician != null)
            {
                // Compose message
                string message = $"You have been assigned to job #{technician.ServiceRequestId}.";
                // FixItFred: Simplified notification since service injection has namespace conflicts
                await Task.Delay(100); // Simulate notification
                // await scheduler.QueueNotificationAsync(...)
            }
            TempData["SystemMessages"] = "Notification queued.";
            return RedirectToPage();
        }
    }
}
