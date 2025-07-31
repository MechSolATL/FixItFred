using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public enum TechnicianBonusType
    {
        FiveStarStreak = 0,
        EmergencySLA = 1,
        PeerlessSLA = 2
    }

    public class TechnicianBonusLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public TechnicianBonusType BonusType { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public int? SourceJobId { get; set; }
        [MaxLength(500)]
        public string? ReasonNote { get; set; }
        public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
    }
}
