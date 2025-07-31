using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ReferralCode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OwnerCustomerId { get; set; } // Referring customer
        [Required]
        [MaxLength(16)]
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public int UsageCount { get; set; } = 0;
        public int? FraudFlagLevel { get; set; } // 0=none, 1=suspicious, 2=blocked
    }

    public class ReferralEventLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ReferralCodeId { get; set; }
        public int? ReferrerCustomerId { get; set; }
        public int? ReferredCustomerId { get; set; }
        [MaxLength(32)]
        public string EventType { get; set; } = string.Empty; // Invite, Signup, Completed, Fraud
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
