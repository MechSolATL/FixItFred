using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class PatchSystemLog
    {
        /// <summary>
        /// The unique identifier for the patch system log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The action performed in the patch system.
        /// </summary>
        [Required]
        public string Action { get; set; } = null!;

        /// <summary>
        /// The user or system that performed the action.
        /// </summary>
        [Required]
        public string PerformedBy { get; set; } = null!;

        /// <summary>
        /// The timestamp when the action was performed.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Additional notes about the action, if applicable.
        /// </summary>
        public string? Notes { get; set; }
    }
}
