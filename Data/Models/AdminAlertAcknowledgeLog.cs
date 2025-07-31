using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    /// <summary>
    /// Tracks admin acknowledgment/mute actions for alerts.
    /// </summary>
    public class AdminAlertAcknowledgeLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AlertId { get; set; }
        [Required]
        public string AdminUserId { get; set; } = string.Empty;
        [Required]
        public string ActionTaken { get; set; } = string.Empty; // e.g. "Acknowledge", "Mute"
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
