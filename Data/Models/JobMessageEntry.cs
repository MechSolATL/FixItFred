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
        /// <summary>
        /// The unique identifier for the job message entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the associated service request.
        /// </summary>
        public int ServiceRequestId { get; set; }

        /// <summary>
        /// The role of the sender (e.g., Technician, Dispatcher).
        /// </summary>
        public string SenderRole { get; set; } = string.Empty;

        /// <summary>
        /// The name of the sender.
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the message was sent.
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Indicates whether the message is an internal note.
        /// </summary>
        public bool IsInternalNote { get; set; }
    }
}
