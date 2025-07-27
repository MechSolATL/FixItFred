using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Tracks loyalty points earned/spent, type, date, and customer/technician reference.
    /// </summary>
    public class LoyaltyPointTransaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public int? TechnicianId { get; set; } // Sprint 84.3: FK to Technician
        public int Points { get; set; }
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty; // Earned, Spent, Bonus, etc.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public int? RelatedReviewId { get; set; } // Link to review if applicable
        [ForeignKey("TechnicianId")]
        public Technician? Technician { get; set; } // Navigation property
    }
}
