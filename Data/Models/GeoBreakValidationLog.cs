using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class GeoBreakValidationLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ValidationTime { get; set; } = DateTime.UtcNow;
        [Required]
        [MaxLength(100)]
        public string LocationStatus { get; set; } = string.Empty; // Stationary, Moving
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int MinutesStationary { get; set; } = 0;
        [Required]
        [MaxLength(500)]
        public string SystemDecision { get; set; } = string.Empty; // Unlock, Block, Delay
        [MaxLength(500)]
        public string? OverrideReason { get; set; }
        [MaxLength(100)]
        public string? Approver { get; set; }
        public bool IsOverride { get; set; } = false;
    }
}
