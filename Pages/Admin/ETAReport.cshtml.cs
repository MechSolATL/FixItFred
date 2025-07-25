// FixItFred Patch Log — Sprint 26.4E
// [2025-07-25T00:00:00Z] — ETAReportModel updated to use shared ETAHistoryEntry model.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Dispatch;
using System;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
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
