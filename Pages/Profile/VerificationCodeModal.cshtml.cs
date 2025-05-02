using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Controllers.Api;
using MVP_Core.Services;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Pages.Profile
{
    public class VerificationCodeModalModel : PageModel
    {
        private readonly EmailVerificationService _emailVerificationService;

        public VerificationCodeModalModel(EmailVerificationService emailVerificationService)
        {
            _emailVerificationService = emailVerificationService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Code is required.")]
        [StringLength(10, ErrorMessage = "Code must not exceed 10 characters.")]
        public string Code { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public void OnGet()
        {
            ViewData["Title"] = "Verify Code – Service Atlanta";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(Email))
            {
                Message = "Invalid input.";
                IsSuccess = false;
                return Page();
            }

            var success = await _emailVerificationService.VerifyCodeAsync(Email, Code);

            Message = success
                ? "✅ Code verified successfully!"
                : "❌ Invalid or expired code. Please try again.";

            IsSuccess = success;

            return Page();
        }
    }
}
