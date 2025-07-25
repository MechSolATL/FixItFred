using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a message exchanged between a technician and dispatcher for a specific service request.
    /// </summary>
    public class TechnicianMessage
    {
        [Key]
        public int MessageId { get; set; }
        public int TechnicianId { get; set; } // Technician involved
        public int RequestId { get; set; } // Related service request
        [Required]
        [MaxLength(20)]
        public string SenderType { get; set; } = "Dispatcher"; // "Tech" or "Dispatcher"
        [Required]
        [MaxLength(2000)]
        public string MessageBody { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool ReadFlag { get; set; } = false; // True if recipient has read
    }
}
