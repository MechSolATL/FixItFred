// Sprint 85.5 — Trust Incident Timeline
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Models;
using Models;
using Data;

namespace Pages.Admin
{
    // Sprint 85.5 — Trust Incident Timeline
    public class TrustTimelineModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public TrustTimelineModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int TechnicianId { get; set; }
        public List<TrustTimelineEntry> TimelineEntries { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (TechnicianId <= 0) return;

            // Gather incidents from TechnicianAlertLog
            var alerts = _db.TechnicianAlertLogs
                .Where(x => x.TechnicianId == TechnicianId)
                .Select(x => new TrustTimelineEntry
                {
                    Date = x.TriggeredAt,
                    Category = "Trust Drop",
                    Note = $"Score dropped from {x.PreviousScore} to {x.CurrentScore}",
                    Severity = x.CurrentScore < x.PreviousScore - 10 ? "High" : "Medium",
                    LinkedLogId = x.Id
                });

            // Gather incidents from CoachingLogEntry
            var coachings = _db.Set<CoachingLogEntry>()
                .Where(x => x.TechnicianId == TechnicianId)
                .Select(x => new TrustTimelineEntry
                {
                    Date = x.Timestamp,
                    Category = $"Coaching: {x.Category}",
                    Note = x.CoachingNote,
                    Severity = "Info",
                    LinkedLogId = x.Id
                });

            // Gather incidents from EscalationLogEntry
            var escalations = _db.EscalationLogs
                .Where(x => x.ScheduleQueueId == TechnicianId) // TODO: Map to TechnicianId if possible
                .Select(x => new TrustTimelineEntry
                {
                    Date = x.CreatedAt,
                    Category = "Escalation",
                    Note = x.Reason ?? x.ActionTaken ?? "Escalation occurred",
                    Severity = "Critical",
                    LinkedLogId = x.Id
                });

            // Combine and order
            TimelineEntries = alerts
                .Concat(coachings)
                .Concat(escalations)
                .OrderByDescending(e => e.Date)
                .ToList();
        }
    }
}
