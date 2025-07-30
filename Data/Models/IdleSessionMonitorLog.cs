using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class IdleSessionMonitorLog
    {
        /// <summary>
        /// The unique identifier for the idle session monitor log.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the idle session.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The timestamp when the idle session started.
        /// </summary>
        public DateTime IdleStartTime { get; set; }

        /// <summary>
        /// The timestamp when the idle session ended, if applicable.
        /// </summary>
        public DateTime? IdleEndTime { get; set; }

        /// <summary>
        /// The duration of the idle session in minutes.
        /// </summary>
        public int IdleMinutes { get; set; } = 0;

        /// <summary>
        /// The system's decision regarding the idle session (e.g., Prompt, Suggest, AutoClockOut).
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string SystemDecision { get; set; } = string.Empty;

        /// <summary>
        /// The reason for overriding the system's decision, if applicable.
        /// </summary>
        [MaxLength(500)]
        public string? OverrideReason { get; set; }

        /// <summary>
        /// The user or system that approved the override, if applicable.
        /// </summary>
        [MaxLength(100)]
        public string? Approver { get; set; }

        /// <summary>
        /// Indicates whether the system's decision was overridden.
        /// </summary>
        public bool IsOverride { get; set; } = false;

        /// <summary>
        /// The timestamp when the technician was clocked out, if applicable.
        /// </summary>
        public DateTime? ClockOutTime { get; set; }
    }
}
