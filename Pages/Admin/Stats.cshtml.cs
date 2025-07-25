using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var etaEntries = _db.ETAHistoryEntries.Where(e => e.PredictedETA != null && e.ActualArrival != null).ToList();
            AvgEtaAccuracy = etaEntries.Any() ? Math.Round(etaEntries.Average(e => Math.Abs(((DateTime)e.PredictedETA - (DateTime)e.ActualArrival).TotalMinutes)), 1) : 0;
            // Weekly ScheduleQueue Volume
            WeekLabels = Enumerable.Range(0, 7).Select(i => now.AddDays(-6 + i).ToString("ddd")).ToList();
            WeekCounts = Enumerable.Range(0, 7).Select(i => _db.ScheduleQueues.Count(q => q.CreatedAt.Date == now.AddDays(-6 + i).Date)).ToList();
            // SignalR Broadcasts (assume NotificationDispatchEngine logs to NotificationsSent)
            SignalRBroadcasts24h = _db.NotificationsSent.Count(n => n.CreatedAt > now.AddHours(-24));
        }
    }
}
