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

        // Sprint 26.6 Patch Log: Added commission and financing properties for admin review and calculation.
        // Commission and financing fields
        public decimal? TechnicianCommission { get; set; } // Sprint 26.6: Commission for assigned tech
        public decimal? FinancingAmount { get; set; } // Sprint 26.6: Amount financed by customer
        public decimal? FinancingAPR { get; set; } // Sprint 26.6: Annual Percentage Rate for financing
        public int? FinancingTermMonths { get; set; } // Sprint 26.6: Financing term in months
        public decimal? FinancingMonthlyPayment { get; set; } // Sprint 26.6: Calculated monthly payment

        // Sprint 47.1 Patch Log: Emergency pledge and fraud log system fields added.
        // Emergency pledge and fraud log fields
        public bool EmergencyPledge { get; set; } // Sprint 47.1: Emergency pledge flag
        public string? EmergencyPledgeNotes { get; set; } // Sprint 47.1: Notes for emergency pledge
        public bool IsFraudSuspected { get; set; } // Sprint 47.1: Suspected fraud flag
        public string? FraudLogNotes { get; set; } // Sprint 47.1: Fraud log notes

        // Sprint ??.
        public DateTime? CompletedDate { get; set; } // New property for dispute tracking

        // Sprint 55.0: ETA property for technician arrival
        public DateTime? EstimatedArrival { get; set; }

        // Sprint 60.0: Timeline PDF archive and visibility
        [MaxLength(500)]
        public string? FinalizedPDFPath { get; set; } // Path to finalized PDF receipt
        public bool ShowInTimeline { get; set; } = true; // Show in customer timeline

        [MaxLength(8000)]
        public string? RoutePlaybackPath { get; set; } // Sprint 61.0: Cached GeoJSON/array for route playback

        // Sprint 69.0: Sync Compliance Enforcer
        public bool HasRequiredMedia { get; set; } = false; // True if required media is present
        public DateTime? SyncComplianceCheckedAt { get; set; } // Last time compliance was checked

        // Sprint 86.0: Dispatcher assignment and ETA fields
        public int? AssignedTechId { get; set; }
        public DateTime? DispatchTime { get; set; }
        public DateTime? ArrivalETA { get; set; }
        public DateTime? ArrivalTime { get; set; } // Sprint 86.7 — Technician AI Companion: Arrival time logging
        public DateTime? InvoiceCompletedAt { get; set; } // Sprint 86.7 — Technician AI Companion: Invoice completion
        public int? CustomerId { get; set; } // Sprint 86.7 — For notification/alert
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
