// Sprint 85.2 — Coaching Logbook System
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    // Sprint 85.2 — Coaching Logbook System
    public class CoachingLogModel : PageModel
    {
        private readonly CoachingLogService _logService;
        public CoachingLogModel(CoachingLogService logService)
        {
            _logService = logService;
        }
        public List<CoachingLogEntry> Entries { get; set; } = new();
        public void OnGet()
        {
            // For now, show all entries (stub)
            Entries = _logService.FilterByDate(System.DateTime.MinValue, System.DateTime.MaxValue);
        }
    }
}
