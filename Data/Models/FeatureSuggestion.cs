using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    // Sprint 83.4: FeatureSuggestion model
    public class FeatureSuggestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public string SuggestionText { get; set; } = string.Empty;
        [Required]
        public string UrgencyLevel { get; set; } = string.Empty;
        [Required]
        public string FeedbackCategory { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string? AttachmentPath { get; set; }
    }
}
