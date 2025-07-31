// FixItFred Patch Log — Sprint 28: Validation Engine
// [2024-07-25T00:30:00Z] — OnPost logic updated for ModelState validation and error feedback.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using System;
using Microsoft.AspNetCore.Mvc;
using Services.Admin;

namespace Pages.Admin
{
    public class SendBroadcastModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        [BindProperty(SupportsGet = true)]
        public string Message { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string ExpiresAt { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public SendBroadcastModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid input.";
                return Page();
            }
            if (string.IsNullOrWhiteSpace(Message))
            {
                ErrorMessage = "Message is required.";
                return Page();
            }
            DateTime? expiresAt = null;
            if (!string.IsNullOrWhiteSpace(ExpiresAt) && DateTime.TryParse(ExpiresAt, out var dt))
                expiresAt = dt;
            _dispatcherService.AddBroadcast(Message, User?.Identity?.Name ?? "system"); // Sprint 83.4: CS7036 fix – required parameter added
            SuccessMessage = "Broadcast sent.";
            return Page();
        }
    }
}
