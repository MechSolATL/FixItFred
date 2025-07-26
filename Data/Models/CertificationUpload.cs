using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class CertificationUpload
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public string FilePath { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public string? VerifiedBy { get; set; }
        public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Rejected, Expired
        public string CertificationName { get; set; } = string.Empty;
        public bool IsExpired => ExpiryDate != null && ExpiryDate < DateTime.UtcNow;
        public string DocumentPath => $"/uploads/certs/{System.IO.Path.GetFileName(FilePath)}";
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; } = DateTime.MinValue;
        public bool IsVerified => VerificationStatus == "Verified";
        public int DaysUntilExpiry => ExpiryDate.HasValue ? (ExpiryDate.Value - DateTime.UtcNow).Days : int.MaxValue;
        public bool IsExpiringSoon => ExpiryDate.HasValue && (ExpiryDate.Value - DateTime.UtcNow).TotalDays <= 30 && !IsExpired;
    }
}