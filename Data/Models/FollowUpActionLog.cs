using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Logs automated follow-up actions for engagement, rewards, reviews, and escalation.
    /// </summary>
    public class FollowUpActionLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string ActionType { get; set; } = string.Empty; // Email, SMS, Banner, etc.
        [Required]
        public string TriggerType { get; set; } = string.Empty; // NoReview, UnclaimedReward, BonusNoReschedule, etc.
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Sent, Ignored, Responded
        public DateTime? ResponseAt { get; set; }
        public int? RelatedServiceRequestId { get; set; }
        public int? RelatedRewardId { get; set; }
        public string? EscalationLevel { get; set; } // Gentle, Assertive
        public int? OpenCount { get; set; }
        public int? ClickCount { get; set; }
        public int? ConversionCount { get; set; }
        public string? Notes { get; set; }
    }
}
