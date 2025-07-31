namespace Models.ViewModels
{
    public class ForecastMetricsViewModel
    {
        public int ProjectedRequests { get; set; }
        public int AverageDelayMinutes { get; set; }
        public decimal ProjectedRevenue { get; set; }
        public int TechnicianGap { get; set; }
    }
}
