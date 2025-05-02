using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using MVP_Core.Data;

namespace MVP_Core.Pages.Profile
{
    public class ProfileReviewModel : PageModel
    {
        private readonly ProfileReviewService _profileReviewService;
        private readonly SeoService _seoService;

        public ProfileReviewModel(ProfileReviewService profileReviewService, SeoService seoService)
        {
            _profileReviewService = profileReviewService;
            _seoService = seoService;
        }

        [BindProperty]
        public ProfileReview ProfileData { get; set; } = new();

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("ProfileReview");

            ViewData["Title"] = seo?.Title ?? "Profile Review – Service Atlanta";
            ViewData["MetaDescription"] = seo?.MetaDescription ?? "Set up your profile to continue.";
            ViewData["Keywords"] = seo?.Keywords ?? "profile setup, service atlanta";
            ViewData["Robots"] = seo?.RobotsMeta ?? "noindex, nofollow";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "⚠️ Please correct the errors in the form.";
                IsSuccess = false;
                return Page();
            }

            ProfileData.CreatedAt = DateTime.UtcNow;
            await _profileReviewService.SaveProfileReviewAsync(new Data.Models.ProfileReviewModel
            {
                Username = ProfileData.Username,
                Email = ProfileData.Email,
                PhoneNumber = ProfileData.PhoneNumber,
                VerificationCode = ProfileData.VerificationCode,
                ReviewNotes = ProfileData.ReviewNotes
            });

            Message = "✅ Profile successfully submitted!";
            IsSuccess = true;
            ModelState.Clear(); // optional: clears form after success
            return Page();
        }
    }
}
