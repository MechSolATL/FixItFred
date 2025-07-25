// [FixItFred] Razor stabilization: Add PageModel for SendBroadcast, handle broadcast submission, DI, and error/success messaging.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Models.Admin;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Admin
{
    public class SendBroadcastModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        [BindProperty]
        public string? Message { get; set; }
        [BindProperty]
        public string? ExpiresAt { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public SendBroadcastModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                ErrorMessage = "Message is required.";
                return Page();
            }
            DateTime? expiresAt = null;
            if (!string.IsNullOrWhiteSpace(ExpiresAt) && DateTime.TryParse(ExpiresAt, out var dt))
                expiresAt = dt;
            _dispatcherService.AddBroadcast(new DispatcherBroadcast
            {
                Message = Message,
                IssuedBy = User?.Identity?.Name ?? "system",
                ExpiresAt = expiresAt
            });
            SuccessMessage = "Broadcast sent.";
            return Page();
        }
    }
}
