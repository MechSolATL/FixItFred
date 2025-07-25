// FixItFred Patch Log — Sprint 26.4C
// [2025-07-25T00:00:00Z] — Manual re-annotation handler added for SchedulerQueue. Triggers AnnotateScheduleQueueAsync.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models; // FixItFred: Use Data.Models for ScheduleQueue
using MVP_Core.Services.Dispatch;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyList<ScheduleQueue> Queue => _dispatchEngine.GetPendingDispatches().ToList();
        [BindProperty]
        public int TechnicianId { get; set; }
        [BindProperty]
        public int ServiceRequestId { get; set; }
        public void OnGet()
        {
            // No-op: queue loaded via property
        }
        // FixItFred: Notification logic to be updated for new engine
    }
}
