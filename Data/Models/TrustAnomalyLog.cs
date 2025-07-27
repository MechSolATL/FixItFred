using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TrustAnomalyLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string AnomalyType { get; set; } = string.Empty;
        public DateTime DetectedAt { get; set; }
        public string GraphNodeContext { get; set; } = string.Empty;
        public double AnomalyScore { get; set; } = 0.0;
        public string ReviewedBy { get; set; } = string.Empty;
        public string Status { get; set; } = "Unreviewed";
    }
}
