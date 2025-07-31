// Sprint 85.2 — Coaching Logbook System
// Sprint 85.7 — Admin Log Hardening & Encryption
using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    // Sprint 85.2 — Coaching Logbook System
    // Sprint 85.7 — Admin Log Hardening & Encryption
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
        // Sprint 85.7 — Admin Log Hardening & Encryption
        public bool IsSensitive { get; set; } = false;
    }
}
