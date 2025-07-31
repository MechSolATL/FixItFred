using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Modules.Patch.Models;
using System.Collections.Generic;

namespace MVP_Core.Modules.Patch.Pages
{
    // Sprint93_Start — PatchDashboard.cshtml.cs initialized
    // Sprint93_Fix_B3 — Stripped SEO dependencies for Razor-safe compilation (rebuild planned in Sprint 93)
    // TODO: Reinject ISEOService and SEOModel in Sprint 93_AfterStabilization

    public class PatchDashboardModel : PageModel
    {
        public List<ReviewEntry> Reviews { get; set; }
        public UserRole CurrentUserRole { get; set; }

        public void OnGet()
        {
            Reviews = new List<ReviewEntry>();
            CurrentUserRole = UserRole.Admin; // Example, replace with actual role logic
        }
    }
}