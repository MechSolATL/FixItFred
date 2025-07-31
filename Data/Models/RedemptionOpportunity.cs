using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class RedemptionOpportunity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(50)]
        public string OpportunityType { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(20)]
        public string Status { get; set; } = "Open";
        public DateTime? ResolvedAt { get; set; }
        [MaxLength(500)]
        public string? ResolutionNotes { get; set; }
        public int PointsRequired { get; set; }
    }
}
