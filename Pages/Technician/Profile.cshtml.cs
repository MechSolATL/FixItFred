using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Data.Models;
using Services;

namespace Pages.Technician
{
    public class ProfileModel : PageModel
    {
        private readonly ITechnicianProfileService _profileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TechnicianProfileDto? PatchProfile { get; set; }
        [BindProperty]
        public string? Nickname { get; set; }
        [BindProperty]
        public bool EnableBanterMode { get; set; }
        public string? PatchPreview { get; set; }
        public string? PatchIdentityError { get; set; }
        public int BanterFlagCount { get; set; }

        // [Sprint91_Recovery_P8] Nova Razor binding patch
        public Seo Seo { get; set; } = new Seo();
        public string TierStatus { get; set; } = "Basic";
        public string ViewTitle { get; set; } = "Profile";
        public string? ReturnUrl { get; set; }

        public ProfileModel(ITechnicianProfileService profileService, IHttpContextAccessor httpContextAccessor)
        {
            _profileService = profileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnGetAsync()
        {
            var techId = User.Identity?.Name != null ? int.Parse(User.Identity.Name) : 0;
            PatchProfile = await _profileService.GetProfileAsync(techId);
            Nickname = PatchProfile?.Nickname;
            EnableBanterMode = PatchProfile?.EnableBanterMode ?? false;
            PatchPreview = GetPatchPreview(PatchProfile);
            BanterFlagCount = _httpContextAccessor.HttpContext?.Session.GetInt32("PatchBanterFlagCount") ?? 0;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var techId = User.Identity?.Name != null ? int.Parse(User.Identity.Name) : 0;
            BanterFlagCount = _httpContextAccessor.HttpContext?.Session.GetInt32("PatchBanterFlagCount") ?? 0;
            var result = await _profileService.UpdatePatchIdentityAsync(techId, Nickname, EnableBanterMode);
            if (result.Flagged)
            {
                BanterFlagCount++;
                _httpContextAccessor.HttpContext?.Session.SetInt32("PatchBanterFlagCount", BanterFlagCount);
            }
            if (!result.Success)
            {
                PatchIdentityError = result.Message;
                PatchProfile = await _profileService.GetProfileAsync(techId);
                PatchPreview = GetPatchPreview(PatchProfile);
                return Page();
            }
            // Success
            PatchProfile = await _profileService.GetProfileAsync(techId);
            PatchPreview = GetPatchPreview(PatchProfile);
            PatchIdentityError = null;
            return RedirectToPage();
        }

        private string GetPatchPreview(TechnicianProfileDto? profile)
        {
            if (profile == null) return "";
            var name = !string.IsNullOrWhiteSpace(profile.Nickname) && profile.NicknameApproved ? profile.Nickname : profile.FullName.Split(' ')[0];
            if (profile.EnableBanterMode)
            {
                return $"Ey! {name}, you fix that boiler or you need me to call my cousin Tony?";
            }
            else
            {
                return $"Hello {name}, ready to get to work?";
            }
        }
    }
}
