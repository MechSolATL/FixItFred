// FixItFred Patch Log — Sprint 28: Validation Engine
// [2024-07-25T00:30:00Z] — DataAnnotations injected for Razor validation compliance.
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Models.Admin
{
    public class TechnicianProfileDto
    {
        public int TechnicianId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<string> SkillTags { get; set; } = new();
        public string[] TopZIPs { get; set; } = new string[0];
        public List<string> Comments { get; set; } = new();
        public DateTime LastActive { get; set; }
        public double CloseRate7Days { get; set; }
        public double CloseRate30Days { get; set; }
        public int CallbackCount7Days { get; set; }
        public int TotalJobsLast30Days { get; set; }
        // Sprint 91.21 - Patch Identity Engine
        public string? Nickname { get; set; }
        public bool NicknameApproved { get; set; }
        public bool EnableBanterMode { get; set; }
    }
}
