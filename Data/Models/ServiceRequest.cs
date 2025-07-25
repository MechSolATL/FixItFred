using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Zip { get; set; } = string.Empty;

        [Required]
        public DateTime RequestedAt { get; set; }

        public int? AssignedTechnicianId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = "Normal";

        public int DelayMinutes { get; set; }

        public bool IsEmergency { get; set; }

        public bool IsEscalated { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? EscalatedAt { get; set; }

        [MaxLength(2000)]
        public string Details { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FirstViewedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ClientConfirmedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        [MaxLength(100)]
        public string? AssignedTo { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(100)]
        public string? SessionId { get; set; }

        public bool IsUrgent { get; set; } = false;

        public bool NeedsFollowUp { get; set; } = false;

        // FIXED: Strongly typed one-to-many relationship
        public List<UserResponse> Responses { get; set; } = [];

        [MaxLength(200)]
        public string? RequiredSkills { get; set; } // New property for required skills

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ServiceSubtype { get; set; } = string.Empty;

        // Sprint 42.2 – Ticket Locking
        public bool IsLocked { get; set; } = false;

        // Sprint 42.3 – Customer Satisfaction Rating
        public int? SatisfactionScore { get; set; }

        [MaxLength(2000)]
        public string? CustomerFeedback { get; set; }
    }

    public class SlaSetting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;
        // SLA in hours
        public int SlaHours { get; set; } = 24;
    }

    public class KanbanHistoryLog
    {
        [Key]
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        [MaxLength(50)]
        public string FromStatus { get; set; } = string.Empty;
        [MaxLength(50)]
        public string ToStatus { get; set; } = string.Empty;
        public int? FromIndex { get; set; }
        public int? ToIndex { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("ServiceRequestId")]
        public ServiceRequest? ServiceRequest { get; set; }
    }
}
