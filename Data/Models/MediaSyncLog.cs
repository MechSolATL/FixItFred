using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class MediaSyncLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int? ServiceRequestId { get; set; }
        public string? Zone { get; set; } // Zip or region
        public bool IsSuccess { get; set; }
        public int RetryCount { get; set; }
        public bool TriggeredFromOffline { get; set; }
        public DateTime AttemptedAt { get; set; }
        public DateTime Timestamp { get; set; } // Added for penalty tracker
    }
}
