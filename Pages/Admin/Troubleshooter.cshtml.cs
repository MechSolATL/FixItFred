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
    /// <summary>
    /// Represents the Razor Page model for troubleshooting equipment issues.
    /// </summary>
    public class TroubleshooterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly LLMEngineService _llmService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TroubleshooterModel"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="llmService">The LLM engine service.</param>
        public TroubleshooterModel(ApplicationDbContext db, LLMEngineService llmService)
        {
            _db = db;
            _llmService = llmService;
        }

        /// <summary>
        /// Gets or sets the error code for troubleshooting.
        /// </summary>
        [BindProperty]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the type of equipment being troubleshooted.
        /// </summary>
        [BindProperty]
        public string EquipmentType { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer of the equipment.
        /// </summary>
        [BindProperty]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the AI-suggested fix for the issue.
        /// </summary>
        [BindProperty]
        public string AISuggestedFix { get; set; }

        /// <summary>
        /// Gets or sets the feedback result provided by the technician.
        /// </summary>
        [BindProperty]
        public string FeedbackResult { get; set; }

        /// <summary>
        /// Gets or sets the notes provided by the technician.
        /// </summary>
        [BindProperty]
        public string TechNotes { get; set; }

        /// <summary>
        /// Gets or sets the known fix for the issue.
        /// </summary>
        public KnownFix KnownFix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether feedback has been submitted.
        /// </summary>
        public bool FeedbackSubmitted { get; set; }

        /// <summary>
        /// Handles the GET request for the page.
        /// </summary>
        public async Task OnGetAsync()
        {
            FeedbackSubmitted = false;
        }

        /// <summary>
        /// Handles the POST request for troubleshooting.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            KnownFix = await _db.KnownFixes.FirstOrDefaultAsync(k => k.ErrorCode == ErrorCode && k.EquipmentType == EquipmentType && k.Manufacturer == Manufacturer);
            var prompt = $"Troubleshoot error code '{ErrorCode}' for equipment type '{EquipmentType}'{(string.IsNullOrWhiteSpace(Manufacturer) ? "" : $" by manufacturer '{Manufacturer}'")}. Provide a step-by-step fix.";
            AISuggestedFix = await _llmService.RunPromptAsync(prompt);
            return Page();
        }

        /// <summary>
        /// Handles the POST request for submitting feedback.
        /// </summary>
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
