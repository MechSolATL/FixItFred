using MVP_Core.Data.DTO.Analytics;

namespace MVP_Core.Pages.Admin
{
    public class AdminAnalyticsViewModel
    {
        public TechnicianStatusMetricsDto TechnicianStatus { get; set; } = new();
        public ToolTransferMetricsDto ToolTransfers { get; set; } = new();
        public ZoneAlertHeatmapDto ZoneHeatmap { get; set; } = new();
        public KpiSummaryDto KPIs { get; set; } = new();
    }
}
