// Sprint 91.17 - TroubleshootingBrain
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class TroubleshooterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly LLMEngineService _llmService;
        public TroubleshooterModel(ApplicationDbContext db, LLMEngineService llmService)
        {
            _db = db;
            _llmService = llmService;
        }

        [BindProperty]
        public string ErrorCode { get; set; }
        [BindProperty]
        public string EquipmentType { get; set; }
        [BindProperty]
        public string Manufacturer { get; set; }
        [BindProperty]
        public string AISuggestedFix { get; set; }
        [BindProperty]
        public string FeedbackResult { get; set; }
        [BindProperty]
        public string TechNotes { get; set; }
        public KnownFix KnownFix { get; set; }
        public bool FeedbackSubmitted { get; set; }

        public async Task OnGetAsync()
        {
            FeedbackSubmitted = false;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            KnownFix = await _db.KnownFixes.FirstOrDefaultAsync(k => k.ErrorCode == ErrorCode && k.EquipmentType == EquipmentType && k.Manufacturer == Manufacturer);
            var prompt = $"Troubleshoot error code '{ErrorCode}' for equipment type '{EquipmentType}'{(string.IsNullOrWhiteSpace(Manufacturer) ? "" : $" by manufacturer '{Manufacturer}'")}. Provide a step-by-step fix.";
            AISuggestedFix = await _llmService.RunPromptAsync(prompt);
            return Page();
        }

        public async Task<IActionResult> OnPostFeedbackAsync()
        {
            // Simulate tech ID for demo
            var techId = Guid.NewGuid();
            bool? wasSuccessful = FeedbackResult == "Worked" ? true : FeedbackResult == "DidntWork" ? false : (bool?)null;
            await _llmService.LogTroubleshootingAttemptAsync(techId, $"{ErrorCode}|{EquipmentType}|{Manufacturer}", AISuggestedFix, wasSuccessful, TechNotes);
            if (wasSuccessful == true)
            {
                await _llmService.PromoteKnownFixAsync(ErrorCode, EquipmentType, Manufacturer, AISuggestedFix);
            }
            FeedbackSubmitted = true;
            return Page();
        }
    }
}
