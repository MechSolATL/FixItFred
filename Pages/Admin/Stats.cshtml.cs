using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    // Sprint 32.1 - Admin Metrics Reporting
    [Authorize(Roles = "Admin")]
    public class StatsModel : PageModel
    {
        private readonly ApplicationDbContext _db; // Sprint 79.2
        public int TotalRequests { get; set; } // Sprint 79.2
        public int PendingCount { get; set; } // Sprint 79.2
        public int DispatchedCount { get; set; } // Sprint 79.2
        public int CancelledCount { get; set; } // Sprint 79.2
        public int CompletedCount { get; set; } // Sprint 79.2
        public int ActiveTechsToday { get; set; } // Sprint 79.2
        public int GpsUpdatedToday { get; set; } // Sprint 79.2
        public int JobsAssignedToday { get; set; } // Sprint 79.2
        public double AvgEtaAccuracy { get; set; } // Sprint 79.2
        public int SignalRBroadcasts24h { get; set; } // Sprint 79.2
        public List<string> WeekLabels { get; set; } = new(); // Sprint 79.2
        public List<int> WeekCounts { get; set; } = new(); // Sprint 79.2
        public string? PdfArchivePath { get; set; } // Sprint 79.2
        public bool PdfArchiveExists => System.IO.File.Exists(PdfArchivePath ?? string.Empty); // Sprint 79.2
        public string? PdfArchiveError { get; set; } // Sprint 79.2
        public string? Sprint47PdfArchivePath { get; set; } // Sprint 79.2
        public bool Sprint47PdfArchiveExists => System.IO.File.Exists(Sprint47PdfArchivePath ?? string.Empty); // Sprint 79.2
        public string? Sprint47PdfArchiveError { get; set; } // Sprint 79.2

        public StatsModel(ApplicationDbContext db) { _db = db ?? throw new ArgumentNullException(nameof(db)); } // Sprint 79.2

        public void OnGet()
        {
            var now = DateTime.UtcNow;
            var requests = _db?.ServiceRequests?.ToList() ?? new List<ServiceRequest>(); // Sprint 79.2
            TotalRequests = requests.Count;
            PendingCount = requests.Count(r => r.Status == "Pending");
            DispatchedCount = requests.Count(r => r.Status == "Dispatched");
            CancelledCount = requests.Count(r => r.Status == "Cancelled");
            CompletedCount = requests.Count(r => r.Status == "Completed");
            ActiveTechsToday = _db?.Technicians?.Count(t => t.IsActive && t.EmploymentDate != null && t.EmploymentDate.Value.Date <= now.Date) ?? 0; // Sprint 79.2
            GpsUpdatedToday = _db?.TechTrackingLogs?.Count(l => l.Timestamp > now.Date) ?? 0; // Sprint 79.2
            JobsAssignedToday = _db?.ScheduleQueues?.Count(q => q.CreatedAt > now.Date) ?? 0; // Sprint 79.2
            var etaEntries = _db?.ETAHistoryEntries?.Where(e => e.PredictedETA.HasValue && e.ActualArrival.HasValue).ToList() ?? new List<ETAHistoryEntry>(); // Sprint 79.2
            AvgEtaAccuracy = etaEntries.Any() ? Math.Round(etaEntries.Average(e => Math.Abs((e.PredictedETA!.Value - e.ActualArrival!.Value).TotalMinutes)), 1) : 0; // Sprint 79.2
            WeekLabels = Enumerable.Range(0, 7).Select(i => now.AddDays(-6 + i).ToString("ddd")).ToList();
            WeekCounts = Enumerable.Range(0, 7).Select(i => _db?.ScheduleQueues?.Count(q => q.CreatedAt.Date == now.AddDays(-6 + i).Date) ?? 0).ToList(); // Sprint 79.2
            SignalRBroadcasts24h = _db?.NotificationsSent?.Count(n => n.CreatedAt > now.AddHours(-24)) ?? 0; // Sprint 79.2
            PdfArchivePath = Path.Combine("Docs", "SprintLogs", "Sprint_44_46_ChangeLog.pdf");
            Sprint47PdfArchivePath = Path.Combine("Docs", "SprintLogs", "Sprint_47_ChangeLog.pdf");
        }

        public async Task<IActionResult> OnPostGeneratePdfArchiveAsync()
        {
            string changelogPath = "CHANGELOG.md";
            string outputPath = Path.Combine("Docs", "SprintLogs", "Sprint_44_46_ChangeLog.pdf");
            string sprintRange = "44–46";
            string author = User?.Identity?.Name ?? "admin";
            try
            {
                if (!System.IO.File.Exists(changelogPath))
                {
                    PdfArchiveError = $"Changelog file not found: {changelogPath}";
                    return Page();
                }
                string changelogText = await System.IO.File.ReadAllTextAsync(changelogPath);
                int start = changelogText.IndexOf("Sprint 44");
                int end = changelogText.IndexOf("Sprint 47");
                string sprintText = (start >= 0) ?
                    changelogText.Substring(start, (end > start ? end - start : changelogText.Length - start)) :
                    changelogText;
                await SprintPdfArchive.GenerateSprintChangeLogPdfAsync(sprintText, outputPath, sprintRange, author);
                PdfArchivePath = outputPath;
            }
            catch (System.Exception ex)
            {
                PdfArchiveError = ex.Message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGenerateSprint47PdfArchiveAsync()
        {
            string changelogPath = "CHANGELOG.md";
            string outputPath = Path.Combine("Docs", "SprintLogs", "Sprint_47_ChangeLog.pdf");
            string sprintRange = "47";
            string author = User?.Identity?.Name ?? "admin";
            try
            {
                if (!System.IO.File.Exists(changelogPath))
                {
                    Sprint47PdfArchiveError = $"Changelog file not found: {changelogPath}";
                    return Page();
                }
                string changelogText = await System.IO.File.ReadAllTextAsync(changelogPath);
                int start = changelogText.IndexOf("Sprint 47");
                int end = changelogText.IndexOf("Sprint 48");
                string sprintText = (start >= 0) ?
                    changelogText.Substring(start, (end > start ? end - start : changelogText.Length - start)) :
                    changelogText;
                await SprintPdfArchive.GenerateSprintChangeLogPdfAsync(sprintText, outputPath, sprintRange, author);
                Sprint47PdfArchivePath = outputPath;
                Sprint47PdfArchiveError = null;
            }
            catch (System.Exception ex)
            {
                Sprint47PdfArchiveError = ex.Message;
            }
            return Page();
        }
    }
}
