using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Data.Models
{
    public class ComplianceAlertLog
    {
        [Key]
        public int Id { get; set; }
        public ComplianceAlertType AlertType { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public bool IsAcknowledged { get; set; }
        public string? AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string? RelatedEntityId { get; set; }
        public string? Severity { get; set; }
    }
}
