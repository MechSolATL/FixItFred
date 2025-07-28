// Sprint 85.2 — Resolved FlaggedCustomer Timestamp for Admin Metrics
using MVP_Core.Data;
using System;
using System.Linq;
using MVP_Core.Models;
using System.Threading.Tasks;
using MVP_Core.Data.DTO.Analytics;

namespace MVP_Core.Services.Admin
{
    // Sprint 85.2 — Resolved FlaggedCustomer Timestamp for Admin Metrics
    public class AdminAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public AdminAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }
        public int GetTechniciansFlaggedThisWeek()
        {
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            // Use FlaggedAt as the date field for FlaggedCustomer
            return _db.FlaggedCustomers.Count(f => f.FlaggedAt >= weekAgo);
        }
        public int GetTrustScoreDrops7Days()
        {
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            return _db.TechnicianAlertLogs.Count(a => a.TriggeredAt >= weekAgo);
        }
        public int GetCoachingLogs14Days()
        {
            var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);
            return _db.Set<CoachingLogEntry>().Count(c => c.Timestamp >= twoWeeksAgo);
        }
        public double GetAvgRebuildSuggestionsPerTech()
        {
            var techCount = _db.Technicians.Count();
            if (techCount == 0) return 0;
            var totalSuggestions = _db.Set<TrustRebuildSuggestion>().Count();
            return Math.Round((double)totalSuggestions / techCount, 2);
        }

        public Task<TechnicianStatusMetricsDto> GetTechnicianStatusMetricsAsync()
        {
            // TODO: Implement real logic
            return Task.FromResult(new TechnicianStatusMetricsDto());
        }
        public Task<ToolTransferMetricsDto> GetToolTransferMetricsAsync()
        {
            // TODO: Implement real logic
            return Task.FromResult(new ToolTransferMetricsDto());
        }
        public Task<ZoneAlertHeatmapDto> GetZoneAlertHeatmapAsync()
        {
            // TODO: Implement real logic
            return Task.FromResult(new ZoneAlertHeatmapDto());
        }
        public Task<KpiSummaryDto> GetAggregateKPIsAsync()
        {
            // TODO: Implement real logic
            return Task.FromResult(new KpiSummaryDto());
        }
    }
}
