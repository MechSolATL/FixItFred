using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    /// <summary>
    /// Logs automated follow-up actions for engagement, rewards, reviews, and escalation.
    /// </summary>
    public class FollowUpActionLog
    {
        /// <summary>
        /// The unique identifier for the follow-up action log.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the user associated with the follow-up action.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The type of action performed (e.g., Email, SMS, Banner).
        /// </summary>
        [Required]
        public string ActionType { get; set; } = string.Empty; // Email, SMS, Banner, etc.

        /// <summary>
        /// The type of trigger for the follow-up action (e.g., NoReview, UnclaimedReward).
        /// </summary>
        [Required]
        public string TriggerType { get; set; } = string.Empty; // NoReview, UnclaimedReward, BonusNoReschedule, etc.

        /// <summary>
        /// The timestamp when the follow-up action was triggered.
        /// </summary>
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The status of the follow-up action (e.g., Pending, Sent, Ignored, Responded).
        /// </summary>
        public string Status { get; set; } = "Pending"; // Pending, Sent, Ignored, Responded

        /// <summary>
        /// The timestamp when a response was received for the follow-up action.
        /// </summary>
        public DateTime? ResponseAt { get; set; }

        /// <summary>
        /// The ID of the related service request, if applicable.
        /// </summary>
        public int? RelatedServiceRequestId { get; set; }

        /// <summary>
        /// The ID of the related reward, if applicable.
        /// </summary>
        public int? RelatedRewardId { get; set; }

        /// <summary>
        /// The escalation level of the follow-up action (e.g., Gentle, Assertive).
        /// </summary>
        public string? EscalationLevel { get; set; } // Gentle, Assertive

        /// <summary>
        /// The number of times the follow-up action was opened.
        /// </summary>
        public int? OpenCount { get; set; }

        /// <summary>
        /// The number of times the follow-up action was clicked.
        /// </summary>
        public int? ClickCount { get; set; }

        /// <summary>
        /// The number of conversions resulting from the follow-up action.
        /// </summary>
        public int? ConversionCount { get; set; }

        /// <summary>
        /// Additional notes regarding the follow-up action.
        /// </summary>
        public string? Notes { get; set; }
    }
}
