using Data.DTO.Analytics;

namespace Pages.Admin
{
    public class AdminAnalyticsViewModel
    {
        public TechnicianStatusMetricsDto TechnicianStatus { get; set; } = new();
        public ToolTransferMetricsDto ToolTransfers { get; set; } = new();
        public ZoneAlertHeatmapDto ZoneHeatmap { get; set; } = new();
        public KpiSummaryDto KPIs { get; set; } = new();
    }
}
