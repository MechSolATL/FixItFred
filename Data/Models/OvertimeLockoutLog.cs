using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class OvertimeLockoutLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime EventTime { get; set; } = DateTime.UtcNow;
        [Required]
        [MaxLength(100)]
        public string EventType { get; set; } = string.Empty; // Lockout, Warning, Override
        [Required]
        [MaxLength(500)]
        public string SystemDecision { get; set; } = string.Empty; // Lockout, Prompt, Delay
        [MaxLength(500)]
        public string? OverrideReason { get; set; }
        [MaxLength(100)]
        public string? Approver { get; set; }
        public bool IsOverride { get; set; } = false;
        public DateTime? ClockOutTime { get; set; }
    }
}
