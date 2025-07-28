using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVP_Core.Data.Enums;

namespace MVP_Core.Data.Models
{
    public class BillingInvoiceRecord
    {
        [Key]
        public Guid Id { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool IsPaid { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime? PaidDate { get; set; }
        public string ExternalTxnId { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public PaymentMethodEnum PaymentMethod { get; set; }
        public bool ConfirmedByAdmin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public int? ServiceRequestId { get; set; }
        [ForeignKey("ServiceRequestId")]
        public ServiceRequest? ServiceRequest { get; set; }

        // Legacy/compatibility fields for PDF/Estimate/Archive
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = "USD";
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string LinkUrl { get; set; } = string.Empty;
        public DateTime RetrievedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsAcknowledged { get; set; }
        public string? AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedDate { get; set; }
        public string? CustomerSignaturePath { get; set; }
        public bool WasAccepted { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? SignatureAuditLog { get; set; }
        public bool WasExportedForLegalReview { get; set; }
        public DateTime? ArchivedAt { get; set; }
        [MaxLength(500)]
        public string? DownloadUrl { get; set; }

        [NotMapped]
        public decimal AmountDue => AmountTotal - (AmountPaid ?? 0);
    }
}
