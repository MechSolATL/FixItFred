// FixItFred – Sprint 44 Build Restoration
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a message in a job/service request thread.
    /// </summary>
    public class JobMessageEntry
    {
        [Key]
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsInternalNote { get; set; }
    }
}
