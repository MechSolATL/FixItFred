namespace MVP_Core.Pages.Admin
{
    public class TechnicianHeatScoreDto
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public double HeatScore { get; set; }
        public DateTime CalculatedAt { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
    }
}