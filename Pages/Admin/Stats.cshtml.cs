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
        private readonly ApplicationDbContext _db;
        public int TotalRequests { get; set; }
        public int PendingCount { get; set; }
        public int DispatchedCount { get; set; }
        public int CancelledCount { get; set; }
        public int CompletedCount { get; set; }
        public int ActiveTechsToday { get; set; }
        public int GpsUpdatedToday { get; set; }
        public int JobsAssignedToday { get; set; }
        public double AvgEtaAccuracy { get; set; }
        public int SignalRBroadcasts24h { get; set; }
        public List<string> WeekLabels { get; set; } = new();
        public List<int> WeekCounts { get; set; } = new();
        // Sprint 47.3 – PDF Archive UI Trigger
        public string? PdfArchivePath { get; set; }
        public bool PdfArchiveExists => System.IO.File.Exists(PdfArchivePath ?? "");
        public string? PdfArchiveError { get; set; }
        public string? Sprint47PdfArchivePath { get; set; }
        public bool Sprint47PdfArchiveExists => System.IO.File.Exists(Sprint47PdfArchivePath ?? "");
        public string? Sprint47PdfArchiveError { get; set; }

        public StatsModel(ApplicationDbContext db) { _db = db; }

        public void OnGet()
        {
            var now = DateTime.UtcNow;
            // Service Request Metrics
            var requests = _db.ServiceRequests.ToList();
            TotalRequests = requests.Count;
            PendingCount = requests.Count(r => r.Status == "Pending");
            DispatchedCount = requests.Count(r => r.Status == "Dispatched");
            CancelledCount = requests.Count(r => r.Status == "Cancelled");
            CompletedCount = requests.Count(r => r.Status == "Completed");
            // Technician Activity
            ActiveTechsToday = _db.Technicians.Count(t => t.IsActive && t.EmploymentDate != null && t.EmploymentDate.Value.Date <= now.Date);
            GpsUpdatedToday = _db.TechTrackingLogs.Count(l => l.Timestamp > now.Date);
            JobsAssignedToday = _db.ScheduleQueues.Count(q => q.CreatedAt > now.Date);
            // ETA Accuracy
            var etaEntries = _db.ETAHistoryEntries.Where(e => e.PredictedETA.HasValue && e.ActualArrival.HasValue).ToList(); // Fix: Use .HasValue for nullable DateTime
            AvgEtaAccuracy = etaEntries.Any() ? Math.Round(etaEntries.Average(e => Math.Abs((e.PredictedETA!.Value - e.ActualArrival!.Value).TotalMinutes)), 1) : 0;
            // Weekly ScheduleQueue Volume
            WeekLabels = Enumerable.Range(0, 7).Select(i => now.AddDays(-6 + i).ToString("ddd")).ToList();
            WeekCounts = Enumerable.Range(0, 7).Select(i => _db.ScheduleQueues.Count(q => q.CreatedAt.Date == now.AddDays(-6 + i).Date)).ToList();
            // SignalR Broadcasts (assume NotificationDispatchEngine logs to NotificationsSent)
            SignalRBroadcasts24h = _db.NotificationsSent.Count(n => n.CreatedAt > now.AddHours(-24));
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
