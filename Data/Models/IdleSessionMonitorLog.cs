using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class IdleSessionMonitorLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime IdleStartTime { get; set; }
        public DateTime? IdleEndTime { get; set; }
        public int IdleMinutes { get; set; } = 0;
        [Required]
        [MaxLength(500)]
        public string SystemDecision { get; set; } = string.Empty; // Prompt, Suggest, AutoClockOut
        [MaxLength(500)]
        public string? OverrideReason { get; set; }
        [MaxLength(100)]
        public string? Approver { get; set; }
        public bool IsOverride { get; set; } = false;
        public DateTime? ClockOutTime { get; set; }
    }
}
