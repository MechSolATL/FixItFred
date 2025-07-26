using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Defines reward tier rules, thresholds, and bonuses for loyalty system.
    /// </summary>
    public class RewardTier
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int PointsRequired { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public int BonusPoints { get; set; } = 0;
        public string? BadgeIcon { get; set; } // Path or name for badge icon
        public bool IsActive { get; set; } = true;
    }
}
