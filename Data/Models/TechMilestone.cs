using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    // Sprint 87.1 — Defines each unlockable achievement, badge, or level
    public class TechMilestone
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int? Points { get; set; }
        public string? Category { get; set; } // e.g. "Badge", "Level", "Title"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
