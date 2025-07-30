using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class CertificationRecord
    {
        /// <summary>
        /// The unique identifier for the certification record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the certification.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The name of the certification.
        /// </summary>
        [Required]
        public string CertificationName { get; set; } = string.Empty;

        /// <summary>
        /// The license number of the certification, if applicable.
        /// </summary>
        public string? LicenseNumber { get; set; }

        /// <summary>
        /// The date when the certification was issued.
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// The date when the certification expires.
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// The file path to the certification document, if available.
        /// </summary>
        public string? DocumentPath { get; set; }

        /// <summary>
        /// Indicates whether the certification has been verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Indicates whether the certification is expired.
        /// </summary>
        public bool IsExpired => ExpiryDate < DateTime.UtcNow;

        /// <summary>
        /// The timestamp when the certification record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}