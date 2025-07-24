using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int? AssignedTechnicianId { get; set; } // <-- Added for Blazor binding

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public bool IsUrgent { get; set; } = false;
        public bool NeedsFollowUp { get; set; } = false;

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(100)]
        public string? SessionId { get; set; }

        // Priority: Low, Normal, High
        [MaxLength(20)]
        public string Priority { get; set; } = "Normal";

        // ? FIXED: Strongly typed one-to-many relationship
        public List<UserResponse> Responses { get; set; } = [];

        // SLA/Deadline
        public DateTime? DueDate { get; set; } // Nullable due date for SLA

        public DateTime? EscalatedAt { get; set; } // Set when SLA escalation occurs

        public bool IsEscalated { get; set; } = false;

        [MaxLength(200)]
        public string? RequiredSkills { get; set; } // New property for required skills
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
