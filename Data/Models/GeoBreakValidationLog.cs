using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class GeoBreakValidationLog
    {
        /// <summary>
        /// The unique identifier for the geo-break validation log.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the validation.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The timestamp when the validation occurred.
        /// </summary>
        public DateTime ValidationTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The status of the technician's location (e.g., Stationary, Moving).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LocationStatus { get; set; } = string.Empty;

        /// <summary>
        /// The latitude of the technician's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude of the technician's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The number of minutes the technician has been stationary.
        /// </summary>
        public int MinutesStationary { get; set; } = 0;

        /// <summary>
        /// The system's decision regarding the geo-break (e.g., Unlock, Block, Delay).
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
    }
}
