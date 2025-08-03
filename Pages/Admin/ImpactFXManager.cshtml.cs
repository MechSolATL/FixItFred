using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;
using Data.Models;
using Services;
using Microsoft.EntityFrameworkCore;

namespace Pages.Admin
{
    /// <summary>
    /// HeroFX Studio - Impact FX Manager for controlling visual effects
    /// Sprint127_HeroFX_StudioDivision - Take Control & Express Yourself
    /// </summary>
    public class ImpactFXManagerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly HeroFXEngine _heroFxEngine;
        private readonly ILogger<ImpactFXManagerModel> _logger;

        public ImpactFXManagerModel(ApplicationDbContext context, HeroFXEngine heroFxEngine, ILogger<ImpactFXManagerModel> logger)
        {
            _context = context;
            _heroFxEngine = heroFxEngine;
            _logger = logger;
        }

        [BindProperty]
        public HeroImpactEffect Effect { get; set; } = new();

        [BindProperty]
        public string SelectedEffectName { get; set; } = string.Empty;

        [BindProperty]
        public bool RandomizerEnabled { get; set; } = false;

        [BindProperty]
        public bool SoundEnabled { get; set; } = true;

        [BindProperty]
        public bool VoiceEnabled { get; set; } = true;

        public List<HeroImpactEffect> Effects { get; set; } = new();
        public object? AnalyticsData { get; set; }
        public string AdminUserId => User?.Identity?.Name ?? "admin";
        public bool IsEffectsSeeded { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Load all effects
                Effects = await _heroFxEngine.GetEffectsAsync(activeOnly: false);
                IsEffectsSeeded = Effects.Any();

                // Load analytics for the last 30 days
                var fromDate = DateTime.UtcNow.AddDays(-30);
                var toDate = DateTime.UtcNow;
                AnalyticsData = await _heroFxEngine.GetAnalyticsAsync(fromDate, toDate);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading HeroFX Manager page");
                TempData["Error"] = "Error loading effects data.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSaveEffectAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            try
            {
                await _heroFxEngine.SaveEffectAsync(Effect, AdminUserId);
                TempData["Success"] = $"Effect '{Effect.DisplayName}' saved successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving effect {EffectName}", Effect.Name);
                ModelState.AddModelError("", "Error saving effect: " + ex.Message);
                await LoadDataAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteEffectAsync(int effectId)
        {
            try
            {
                var success = await _heroFxEngine.DeleteEffectAsync(effectId, AdminUserId);
                if (success)
                {
                    TempData["Success"] = "Effect deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Effect not found.";
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting effect {EffectId}", effectId);
                TempData["Error"] = "Error deleting effect: " + ex.Message;
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostSeedDefaultEffectsAsync()
        {
            try
            {
                await _heroFxEngine.SeedDefaultEffectsAsync(AdminUserId);
                TempData["Success"] = "Default effects seeded successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default effects");
                TempData["Error"] = "Error seeding effects: " + ex.Message;
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostPreviewEffectAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedEffectName))
                {
                    return new JsonResult(new { success = false, message = "No effect selected" });
                }

                var success = await _heroFxEngine.TriggerEffectAsync(SelectedEffectName, "preview", AdminUserId, "Admin", "desktop");
                return new JsonResult(new { success, effectName = SelectedEffectName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing effect {EffectName}", SelectedEffectName);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetEffectAsync(int id)
        {
            try
            {
                var effect = await _heroFxEngine.GetEffectAsync(id);
                if (effect == null)
                {
                    return new JsonResult(new { success = false, message = "Effect not found" });
                }
                return new JsonResult(new { success = true, effect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading effect {EffectId}", id);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostTriggerRandomEffectAsync()
        {
            try
            {
                var effect = await _heroFxEngine.GetRandomEffectAsync();
                if (effect == null)
                {
                    return new JsonResult(new { success = false, message = "No effects available" });
                }

                var success = await _heroFxEngine.TriggerEffectAsync(effect.Name, "random", AdminUserId, "Admin", "desktop");
                return new JsonResult(new { success, effectName = effect.Name, displayName = effect.DisplayName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering random effect");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostLogReactionAsync(string effectName)
        {
            try
            {
                await _heroFxEngine.LogReactionAsync(effectName, AdminUserId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging reaction for {EffectName}", effectName);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        private async Task LoadDataAsync()
        {
            Effects = await _heroFxEngine.GetEffectsAsync(activeOnly: false);
            
            var fromDate = DateTime.UtcNow.AddDays(-30);
            var toDate = DateTime.UtcNow;
            AnalyticsData = await _heroFxEngine.GetAnalyticsAsync(fromDate, toDate);
        }
    }
}