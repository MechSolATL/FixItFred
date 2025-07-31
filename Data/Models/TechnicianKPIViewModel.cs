using System;

namespace Data.Models
{
    public class TechnicianKPIViewModel
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public double AvgResponseTime { get; set; }
        public double CompletionRate { get; set; }
        public decimal TotalEarnings { get; set; }
        public int ActiveCertifications { get; set; }
        public double CustomerRating { get; set; }
    }
}
