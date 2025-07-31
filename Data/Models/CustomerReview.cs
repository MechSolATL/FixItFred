using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    /// <summary>
    /// Stores customer feedback, rating, and job reference for loyalty/review system.
    /// </summary>
    public class CustomerReview
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int ServiceRequestId { get; set; }
        // Sprint 84.6 — Review Display Integration
        public int? TechnicianId { get; set; } // Optional technician reference
        [Range(1,5)]
        public int Rating { get; set; }
        [MaxLength(2000)]
        public string? Feedback { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false; // Sprint 84.6 — Review Display Integration
        public bool IsGamifiedBonus { get; set; } = false; // For random bonus events
        public string? BadgeAwarded { get; set; } // For gamified badges
        public float? SentimentScore { get; set; } // Sprint 57.0: Sentiment analysis score
        public string? Keywords { get; set; } // Sprint 57.0: Extracted keywords
        public bool IsFlagged { get; set; } = false; // Sprint 57.0: Flag for concerning feedback
        // Sprint 84.7.2 — Live Filter + UI Overlay
        public bool IsApproved { get; set; } = true;
        public string? Tier { get; set; }
        public string? CustomerName { get; set; }
        public string? Comment { get; set; }
        public DateTime? Timestamp => SubmittedAt;
    }
}
