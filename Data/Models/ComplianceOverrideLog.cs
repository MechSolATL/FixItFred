using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ComplianceOverrideLog
    {
        [Key]
        public int Id { get; set; }
        public Guid TenantId { get; set; }
        [Required]
        public string DocumentType { get; set; } = string.Empty;
        [Required]
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? AdminNote { get; set; }
    }
}
