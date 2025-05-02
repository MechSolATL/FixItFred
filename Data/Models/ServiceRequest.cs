using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ServiceSubtype { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Details { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FirstViewedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ClientConfirmedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        [MaxLength(100)]
        public string? AssignedTo { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public bool IsUrgent { get; set; } = false;
        public bool NeedsFollowUp { get; set; } = false;

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(100)]
        public string? SessionId { get; set; }
    }
}
