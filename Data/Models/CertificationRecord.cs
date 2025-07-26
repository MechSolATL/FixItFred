using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class CertificationRecord
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        [Required]
        public string CertificationName { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? DocumentPath { get; set; }
        public bool IsVerified { get; set; }
        public bool IsExpired => ExpiryDate < DateTime.UtcNow;
        public DateTime CreatedAt { get; set; }
    }
}