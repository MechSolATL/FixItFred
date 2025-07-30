using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a message exchanged between a technician and dispatcher for a specific service request.
    /// </summary>
    public class TechnicianMessage
    {
        /// <summary>
        /// The unique identifier for the message.
        /// </summary>
        [Key]
        public int MessageId { get; set; }

        /// <summary>
        /// The ID of the technician involved in the message exchange.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The ID of the related service request.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// The type of sender (e.g., "Tech" or "Dispatcher").
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string SenderType { get; set; } = "Dispatcher";

        /// <summary>
        /// The body of the message.
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string MessageBody { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the message was sent.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates whether the recipient has read the message.
        /// </summary>
        public bool ReadFlag { get; set; } = false;
    }
}
