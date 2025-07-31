// Sprint 84.0 — OnboardingStatus Schema
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    // Sprint 84.0 — OnboardingStatus Schema
    public class UserOnboardingStatus
    {
        [Key]
        public Guid UserId { get; set; } // FK to Customers or AdminUser
        public bool IsProsCertified { get; set; }
        public DateTime? CertifiedOn { get; set; }
        public string? CertifiedBy { get; set; }
        public bool OnboardingComplete { get; set; }
        public string? ChecklistStatus { get; set; } // nullable json blob for future use
    }
}
