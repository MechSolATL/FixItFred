using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Logs customer bonuses earned from promotions and loyalty events.
    /// </summary>
    public class CustomerBonusLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        [Required]
        public int PromotionId { get; set; }
        [MaxLength(50)]
        public string RewardType { get; set; } = string.Empty;
        public DateTime DateEarned { get; set; } = DateTime.UtcNow;
        public DateTime? DateClaimed { get; set; }
        public bool IsClaimed { get; set; } = false;
        [MaxLength(50)]
        public string SourceTrigger { get; set; } = string.Empty;
        public int? LoyaltyPointsAwarded { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string? ClaimCode { get; set; }
    }
}
