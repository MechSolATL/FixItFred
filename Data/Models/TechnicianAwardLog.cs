using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class TechnicianAwardLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        [MaxLength(50)]
        public string AwardType { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(500)]
        public string? Reason { get; set; }
        [MaxLength(100)]
        public string? Issuer { get; set; }
        public int TrustBoostScore { get; set; }
        public int KarmaBoostScore { get; set; }
        [MaxLength(20)]
        public string? AwardLevel { get; set; }
    }
}
