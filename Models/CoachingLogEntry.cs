// Sprint 85.2 — Coaching Logbook System
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Models
{
    // Sprint 85.2 — Coaching Logbook System
    public class CoachingLogEntry
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        [Required]
        public string SupervisorName { get; set; } = string.Empty;
        [Required]
        public string CoachingNote { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
