// Sprint 85.2 — Admin Metrics Dashboard Integration
namespace MVP_Core.Models
{
    public class AdminMetricsViewModel
    {
        public int TechniciansFlaggedThisWeek { get; set; }
        public int TrustScoreDrops7Days { get; set; }
        public int CoachingLogs14Days { get; set; }
        public double AvgRebuildSuggestionsPerTech { get; set; }
    }
}
