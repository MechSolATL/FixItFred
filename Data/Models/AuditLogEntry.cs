// Sprint 32.2 - Security + Audit Harden
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class AuditLogEntry
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? AdditionalData { get; set; }
    }
}
