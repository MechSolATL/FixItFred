// FixItFred Patch Log — Sprint 26.2C
// [2025-07-25T00:00:00Z] — Resolved TechnicianProfileDto namespace ambiguity with explicit qualification.
// FixItFred Patch Log — Sprint 26.2B
// [2025-07-25T00:00:00Z] — Resolved invalid ServiceRequest namespace reference and fixed DispatcherService usage in SubmitFeedbackModel.
// FixItFred Patch Log — CS0234 PageModel Addition
// 2024-07-24T21:22:00Z
// Applied Fixes: CS0234
// Notes: Added missing SubmitFeedbackModel PageModel for Razor binding compatibility.
// FixItFred Patch Log — Sprint 26
// [2024-07-25T00:15:00Z] — Admin panel hardened with null guards, model binding checks, and error-proof logic.
// FixItFred Patch Log — Sprint 26.2B
// [2024-07-25T00:30:00Z] — TechnicianDropdownViewModel property and population for Razor partial injection.
// FixItFred Patch Log — Sprint 29A Recovery
// [2025-07-25T00:00:00Z] — Added RequestId, Rating, and Notes properties for feedback form binding.
// FixItFred — Sprint 46.1 Build Correction + Compliance
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models.ViewModels;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class SubmitFeedbackModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;

        public SubmitFeedbackModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        [BindProperty(SupportsGet = true)]
        public string Feedback { get; set; } = string.Empty;

        [BindProperty]
        public int RequestId { get; set; }
        [BindProperty]
        public int Rating { get; set; }
        [BindProperty]
        public string Notes { get; set; } = string.Empty;

        public TechnicianDropdownViewModel TechnicianDropdownViewModel { get; set; } = new();
        public List<ServiceRequest> CompletedJobs { get; set; } = new(); // Corrected type

        public void OnGet()
        {
            // Populate Feedback from service or database as needed
            Feedback = Feedback ?? string.Empty;

            // Populate technician dropdown
            var heartbeats = _dispatcherService.GetAllTechnicianHeartbeats();
            // FixItFred — Sprint 46.1 Build Correction + Compliance: Use TechnicianStatusDto directly
            TechnicianDropdownViewModel = new TechnicianDropdownViewModel
            {
                Technicians = heartbeats,
                SelectedTechnicianId = null // or set from query/context
            };

            // Populate completed jobs if needed
            CompletedJobs = new List<ServiceRequest>(); // Replace with actual job fetch logic
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Optionally set error message or handle invalid state
                return Page();
            }
            Feedback = Feedback ?? string.Empty;
            // Save feedback logic here, ensure null guards
            return RedirectToPage("/Admin/ViewFeedback");
        }
    }
}
