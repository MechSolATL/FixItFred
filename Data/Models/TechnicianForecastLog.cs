using System;

namespace Data.Models
{
    public class TechnicianForecastLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ForecastDate { get; set; }
        public decimal ProjectedJobs { get; set; }
        public decimal ExpectedScore { get; set; }
        public string ForecastNotes { get; set; } = string.Empty;
    }
}
