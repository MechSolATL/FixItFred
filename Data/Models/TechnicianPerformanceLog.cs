using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// TechnicianPerformanceLog: Logs compliance failures and other incidents for technician performance tracking.
    /// </summary>
    public class TechnicianPerformanceLog
    {
        /// <summary>
        /// The unique identifier for the performance log entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the performance log entry.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The ID of the service request associated with the performance log entry.
        /// </summary>
        public int ServiceRequestId { get; set; }

        /// <summary>
        /// The type of incident logged (e.g., compliance failure, delay).
        /// </summary>
        [MaxLength(100)]
        public string IncidentType { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the incident.
        /// </summary>
        [MaxLength(2000)]
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the incident occurred.
        /// </summary>
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
