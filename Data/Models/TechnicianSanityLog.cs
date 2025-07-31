using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
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
        // Sprint 83.7-Hardening: Added missing 'SanityCategory' column to TechnicianSanityLogs
        public string? SanityCategory { get; set; }
    }
}
