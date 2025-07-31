// FixItFred Patch Log — Sprint 26.4E
// [2025-07-25T00:00:00Z] — ETAReportModel updated to use shared ETAHistoryEntry model.
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Dispatch;
using System;
using System.Collections.Generic;

namespace Pages.Admin
{
    public class ETAReportModel : PageModel
    {
        private readonly NotificationDispatchEngine _dispatchEngine;
        public ETAReportModel(NotificationDispatchEngine dispatchEngine)
        {
            _dispatchEngine = dispatchEngine;
        }
        public List<ETAHistoryEntry> ETAHistory { get; private set; } = new();
        public void OnGet()
        {
            // FixItFred: Implement GetETAHistory or similar logic in NotificationDispatchEngine as needed
            // ETAHistory = _dispatchEngine.GetETAHistory();
        }
    }
}
