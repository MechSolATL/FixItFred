using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? Role { get; set; }

        public DateTime? LastProfileReviewDate { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? ReviewNotes { get; set; }

        // Sprint 86.4 — Role-Based Feature Toggle Engine
        public List<string> EnabledModules { get; set; } = new();
        public string JobFunction { get; set; } = string.Empty;  // e.g. "TechnicianLead", "Dispatcher", "Viewer"
        public int SkillLevel { get; set; }      // 1 = Entry, 2 = Mid, 3 = Lead
        public int TrustIndex { get; set; } = 0; // Sprint 86.6 — Mentor Routing: TrustIndex for mentor selection
    }
}
