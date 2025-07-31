using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class DisputeRecord
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CustomerEmail { get; set; } = string.Empty;
        [Required]
        public int ServiceRequestId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty; // Billing, Technician, Outcome
        public string Details { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Open"; // Open, Under Review, Resolved, Rejected
        public string ResolutionNotes { get; set; } = string.Empty;
        public string ReviewedBy { get; set; } = string.Empty;
        public int EscalationLevel { get; set; } = 0;
        public string? AttachmentPath { get; set; }
    }
}
