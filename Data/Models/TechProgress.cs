using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    // Sprint 87.1 — Tracks each technician’s current progress, counts, and unlock status
    public class TechProgress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public int MilestoneId { get; set; }
        public int ProgressCount { get; set; } = 0;
        public bool IsUnlocked { get; set; } = false;
        public DateTime? UnlockedAt { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
