using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// TechnicianPerformanceLog: Logs compliance failures and other incidents for technician performance tracking.
    /// </summary>
    public class TechnicianPerformanceLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int ServiceRequestId { get; set; }
        [MaxLength(100)]
        public string IncidentType { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string Details { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
