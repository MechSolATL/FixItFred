using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Log for second-chance technician flagging, review, and override actions.
    /// </summary>
    public class SecondChanceFlagLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        [MaxLength(100)]
        public string TriggeredBy { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? ReviewedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsOverrideApproved { get; set; } = false;
        [MaxLength(500)]
        public string? OverrideNotes { get; set; }
    }
}
