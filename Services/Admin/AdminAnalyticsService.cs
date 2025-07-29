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
            // Demo/mock data for grid render
            return Task.FromResult(new TechnicianStatusMetricsDto
            {
                Assigned = 12,
                EnRoute = 7,
                Idle = 3
            });
        }
        public Task<ToolTransferMetricsDto> GetToolTransferMetricsAsync()
        {
            return Task.FromResult(new ToolTransferMetricsDto
            {
                Pending = 5,
                InTransit = 2,
                Completed = 18
            });
        }
        public Task<ZoneAlertHeatmapDto> GetZoneAlertHeatmapAsync()
        {
            return Task.FromResult(new ZoneAlertHeatmapDto
            {
                Zones = new List<ZoneAlert>
                {
                    new ZoneAlert { ZoneName = "North", AlertCount = 4, Density = 0.8 },
                    new ZoneAlert { ZoneName = "South", AlertCount = 2, Density = 0.3 },
                    new ZoneAlert { ZoneName = "East", AlertCount = 1, Density = 0.1 },
                    new ZoneAlert { ZoneName = "West", AlertCount = 0, Density = 0.0 }
                }
            });
        }
        public Task<KpiSummaryDto> GetAggregateKPIsAsync()
        {
            return Task.FromResult(new KpiSummaryDto
            {
                OverdueJobs = 6,
                IdleTechnicians = 3,
                MissedTransfers = 1,
                TotalTechnicians = 22,
                TotalTransfers = 25
            });
        }
    }
}
