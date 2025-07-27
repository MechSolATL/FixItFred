using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TechnicianSanityLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool EmotionalFatigueFlag { get; set; }
        public int ErrorRateSpike { get; set; }
        public bool BurnoutPatternDetected { get; set; }
        public bool AIInterventionTriggered { get; set; }
        public bool FalseReportShielded { get; set; }
        public string? ShieldingNotes { get; set; }
    }
}
