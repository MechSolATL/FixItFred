using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents a promotion or loyalty event with rules and triggers.
    /// </summary>
    public class PromotionEvent
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(100)]
        public string? ServiceType { get; set; }
        [MaxLength(50)]
        public string? Zone { get; set; }
        [MaxLength(50)]
        public string RewardType { get; set; } = string.Empty; // Points, Discount, Credit
        [MaxLength(50)]
        public string TriggerType { get; set; } = string.Empty; // JobsCompleted, Referrals, ReviewStreak
        public bool Active { get; set; } = true;
        public int MinJobsRequired { get; set; } = 0;
        public int ReferralBonus { get; set; } = 0;
        public int? StreakRequired { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? LoyaltyPointsAwarded { get; set; }
        public bool IsLimitedTime => EndDate > StartDate;
    }
}
