using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System; // Sprint 84.7.2 — Live Filter + UI Overlay

namespace MVP_Core.Data.Models
{
    public class Technician
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? Specialty { get; set; }

        public int CurrentJobCount { get; set; } = 0;
        public int MaxJobCapacity { get; set; } = 5;

        public DateTime? Birthday { get; set; }
        public DateTime? EmploymentDate { get; set; }
        [MaxLength(500)]
        public string? PhotoUrl { get; set; }
        public string? Badges { get; set; } // JSON array of badge names/objects

        // FixItFred: Sprint 30E – Live Tracking: Add GPS fields
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int? DispatchScore { get; set; } // Sprint 35: Technician performance/dispatch score

        // Sprint 39: Skill-based tags for technician specialization
        [MaxLength(1000)]
        public string? SkillTags { get; set; } // Comma-separated skill tags (e.g., "Tankless,Mini Split,Backflow Cert")

        public decimal? HourlyRate { get; set; } // Hourly rate for pay calculation

        // Sprint 51.1: Extended Technician model fields
        public bool IsSecondChance { get; set; } = false; // Second-chance tech flag
        public bool RequiresSupervision { get; set; } = false; // Supervision required
        [MaxLength(50)]
        public string? OnboardingStatus { get; set; } // Onboarding status (e.g., Pending, Approved, Blocked)

        // Sprint 84.5: Leaderboard enhancements
        public int TierLevel { get; set; } = 0; // Leaderboard tier
        public int TotalPoints { get; set; } = 0; // Leaderboard points
        [MaxLength(100)]
        public string City { get; set; } = string.Empty; // Leaderboard city

        public string Name => FullName;

        // Sprint 84.7.2 — Live Filter + UI Overlay
        public int TrustScore { get; set; } = 100;

        // Sprint 84.8 — Technician Heat Score + Map Overlay
        public int HeatScore { get; set; } = 100;
        public string ZipCode { get; set; } = string.Empty;
        public DateTime? LastReviewedAt { get; set; }
    }
}
