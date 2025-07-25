// FixItFred Patch Log — Sprint 28: Validation Engine
// [2024-07-25T00:30:00Z] — DataAnnotations injected for Razor validation compliance.
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Models.Admin
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
    }
}
