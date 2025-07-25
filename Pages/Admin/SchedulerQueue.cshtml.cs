// FixItFred Patch Log — Sprint 26.4C
// [2025-07-25T00:00:00Z] — Manual re-annotation handler added for SchedulerQueue. Triggers AnnotateScheduleQueueAsync.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Dispatch;
using MVP_Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class SchedulerQueueModel : PageModel
    {
        private readonly NotificationDispatchEngine _dispatchEngine;
        public SchedulerQueueModel(NotificationDispatchEngine dispatchEngine)
        {
            _dispatchEngine = dispatchEngine;
        }
        public IReadOnlyList<ScheduleQueue> Queue => _dispatchEngine.GetQueue();
        [BindProperty]
        public int TechnicianId { get; set; }
        [BindProperty]
        public int ServiceRequestId { get; set; }
        public void OnGet()
        {
            // No-op: queue loaded via property
        }
        public IActionResult OnPostNotify()
        {
            // Simulate notification
            _dispatchEngine.MarkAsNotified(TechnicianId, ServiceRequestId, "Manual", $"Manual notification triggered for tech {TechnicianId}, request {ServiceRequestId}");
            TempData["SystemMessages"] = $"Notification sent to technician {TechnicianId} for request {ServiceRequestId}.";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostReannotateAsync()
        {
            var queue = new List<ScheduleQueue>(_dispatchEngine.GetQueue());
            await _dispatchEngine.AnnotateScheduleQueueAsync(queue);
            TempData["SystemMessages"] = "Queue re-annotated with latest technician status and ETA.";
            return RedirectToPage();
        }
    }
}
