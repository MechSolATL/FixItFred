using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Data.DTO.Analytics;
using Services.Admin;

namespace Pages.Admin
{
    public class AnalyticsDashboardModel : PageModel
    {
        private readonly AdminAnalyticsService _analyticsService;
        public AnalyticsDashboardModel(AdminAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public TechnicianStatusMetricsDto TechnicianStatusMetrics { get; set; } = new();
        public ToolTransferMetricsDto ToolTransferMetrics { get; set; } = new();
        public ZoneAlertHeatmapDto ZoneAlertHeatmap { get; set; } = new();
        public KpiSummaryDto KpiSummary { get; set; } = new();

        public async Task OnGetAsync()
        {
            TechnicianStatusMetrics = await _analyticsService.GetTechnicianStatusMetricsAsync();
            ToolTransferMetrics = await _analyticsService.GetToolTransferMetricsAsync();
            ZoneAlertHeatmap = await _analyticsService.GetZoneAlertHeatmapAsync();
            KpiSummary = await _analyticsService.GetAggregateKPIsAsync();
        }
    }
}
