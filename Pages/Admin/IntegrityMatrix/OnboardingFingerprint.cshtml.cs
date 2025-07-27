using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System.Linq;

namespace MVP_Core.Pages.Admin.IntegrityMatrix
{
    public class OnboardingFingerprintModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IntegrityScoringService _scoringService;
        public EmployeeOnboardingProfile? Profile { get; set; }
        public bool HasAccess { get; set; }

        public OnboardingFingerprintModel(ApplicationDbContext db, IntegrityScoringService scoringService)
        {
            _db = db;
            _scoringService = scoringService;
        }

        public IActionResult OnGet(int id)
        {
            // Only allow system super-admins, HR analytics, or AI system roles
            HasAccess = User.IsInRole("SuperAdmin") || User.IsInRole("HRAnalytics") || User.IsInRole("AISystem");
            if (!HasAccess) return Page();
            Profile = _db.Set<EmployeeOnboardingProfile>().FirstOrDefault(p => p.Id == id);
            return Page();
        }

        public IActionResult OnPostFlag(int id)
        {
            HasAccess = User.IsInRole("SuperAdmin") || User.IsInRole("HRAnalytics") || User.IsInRole("AISystem");
            if (!HasAccess) return Page();
            var profile = _db.Set<EmployeeOnboardingProfile>().FirstOrDefault(p => p.Id == id);
            if (profile != null)
            {
                profile.ReviewRequired = true;
                _db.SaveChanges();
            }
            Profile = profile;
            return Page();
        }
    }
}
