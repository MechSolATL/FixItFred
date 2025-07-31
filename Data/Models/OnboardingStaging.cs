using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    // Sprint 83.3: SmartControlUX Gatekeeper Wizard
    public class OnboardingStaging
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public string FilePaths { get; set; } = string.Empty; // Comma-separated
        public bool IsVerified { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
