// FixItFred – Sprint 44 Build Restoration
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DispatcherAuditModel : PageModel
    {
        public AuditStatsDto? AuditStats { get; set; }
        public void OnGet()
        {
            // Dummy data for build restoration
            AuditStats = new AuditStatsDto
            {
                AiSuccessRate = 87.5,
                AiMatchedCount = 70,
                TotalAssignments = 80,
                OverrideRate = 12.5,
                DispatcherOverrideCount = 10,
                TechAvailabilityAtDispatch = 3.2,
                MostCommonOverrideReasons = new List<string> { "Zone Overload", "Manual Preference" }
            };
        }
        public class AuditStatsDto
        {
            public double AiSuccessRate { get; set; }
            public int AiMatchedCount { get; set; }
            public int TotalAssignments { get; set; }
            public double OverrideRate { get; set; }
            public int DispatcherOverrideCount { get; set; }
            public double TechAvailabilityAtDispatch { get; set; }
            public List<string> MostCommonOverrideReasons { get; set; } = new();
        }
    }
}
