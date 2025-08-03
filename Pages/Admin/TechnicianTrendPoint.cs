namespace MVP_Core.Pages.Admin
{
    public class TechnicianTrendPoint
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double Score { get; set; }
        public double HeatScore { get; set; }
        public string TrendDirection { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}