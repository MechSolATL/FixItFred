using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class TechnicianConflictLog
    {
        /// <summary>
        /// The unique identifier for the technician conflict log entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician involved in the conflict.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The ID of the other party involved in the conflict.
        /// </summary>
        public int ConflictWithId { get; set; }

        /// <summary>
        /// The type of conflict (e.g., scheduling, performance).
        /// </summary>
        public string ConflictType { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the conflict occurred.
        /// </summary>
        public DateTime OccurredAt { get; set; }

        /// <summary>
        /// Indicates whether the conflict has been resolved.
        /// </summary>
        public bool Resolved { get; set; } = false;

        /// <summary>
        /// Notes about the resolution of the conflict.
        /// </summary>
        public string ResolutionNotes { get; set; } = string.Empty;

        /// <summary>
        /// The severity level of the conflict.
        /// </summary>
        public int SeverityLevel { get; set; } = 1;

        /// <summary>
        /// The impact of the conflict on trust score.
        /// </summary>
        public double TrustImpactScore { get; set; } = 0.0;
    }
}
