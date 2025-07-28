// Sprint 85.1 — Trust Rebuild Suggestion Engine Core
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Models
{
    // Sprint 85.1 — Trust Rebuild Suggestion Engine Core
    public class TrustRebuildSuggestion
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty; // Behavior, Timeliness, Communication
        [Required]
        public string SuggestedAction { get; set; } = string.Empty;
        public int Weight { get; set; }
        [Required]
        public string RecommendedBy { get; set; } = string.Empty; // System/User/Algorithm
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
