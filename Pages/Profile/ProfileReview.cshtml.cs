// File: MVP-Core/Pages/Profile/ProfileReview.cshtml.cs
using MVP_Core.Services;

namespace MVP_Core.Pages.Profile
{
    [ValidateAntiForgeryToken]
    public class ProfileReviewModel : PageModel
    {
        private readonly ProfileReviewService _profileReviewService;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public ProfileReviewModel(ProfileReviewService profileReviewService, ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _profileReviewService = profileReviewService;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        [BindProperty]
        public ProfileReview ProfileData { get; set; } = new ProfileReview();

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Profile/ProfileReview");
            ViewData["Title"] = seo?.Title ?? "Profile Review";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "?? Please correct the errors in the form.";
                IsSuccess = false;
                return Page();
            }

            ProfileData.CreatedAt = DateTime.UtcNow;

            _profileReviewService.SaveProfileReview(new ProfileReview
            {
                Username = ProfileData.Username,
                Email = ProfileData.Email,
                PhoneNumber = ProfileData.PhoneNumber,
                VerificationCode = ProfileData.VerificationCode,
                ReviewNotes = ProfileData.ReviewNotes,
                CreatedAt = ProfileData.CreatedAt
            });

            await Task.CompletedTask; // ? prevent CS1998 warning

            Message = "? Profile successfully submitted!";
            IsSuccess = true;

            ModelState.Clear();
            return Page();
        }
    }
}
