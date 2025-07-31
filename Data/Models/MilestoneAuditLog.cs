using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    // Sprint 87.1 — Records when milestones were unlocked or updated (audit/history)
    public class MilestoneAuditLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public int MilestoneId { get; set; }
        public string Action { get; set; } = string.Empty; // e.g. "Unlocked", "Progressed"
        public string? Notes { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
