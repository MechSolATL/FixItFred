using System;

namespace MVP_Core.Data.Models
{
    public class TechnicianInsightLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string InsightType { get; set; } = string.Empty;
        public string InsightDetail { get; set; } = string.Empty;
        public DateTime LoggedAt { get; set; }
    }
}
