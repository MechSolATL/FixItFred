using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System;
using System.Threading.Tasks;

namespace MVP_Core.Pages
{
    public class PlumbingModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;

        public PlumbingModel(ApplicationDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            Form = new PlumbingFormModel();  // ✅ Initialize Form
            Message = string.Empty;          // ✅ Initialize Message
        }

        [BindProperty]
        public PlumbingFormModel Form { get; set; }  // ✅ Bound to Form.Email

        [BindProperty]
        public string Message { get; set; } = string.Empty;  // ✅ Avoid null warnings

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(Form.Email))
            {
                Message = "Please enter a valid email.";
                return Page();
            }

            try
            {
                // Generate verification code
                var verificationCode = Guid.NewGuid().ToString();

                // Save to database
                var verification = new EmailVerification
                {
                    Email = Form.Email,
                    VerificationCode = verificationCode,
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.EmailVerifications.Add(verification);
                await _dbContext.SaveChangesAsync();

                // Create verification link (✅ Ensure it's never null)
                var verificationLink = Url.Page(
                    "/Verify",
                    pageHandler: null,
                    values: new { code = verificationCode },
                    protocol: Request.Scheme) ?? throw new InvalidOperationException("Failed to generate verification link.");

                // Send the verification email (✅ Wrapped in try-catch)
                await _emailService.SendVerificationEmailAsync(Form.Email, verificationLink);

                Message = "✅ A verification link has been sent to your email.";
            }
            catch (Exception ex)
            {
                // Log the error (implement logging later)
                Message = $"❌ Failed to send the verification email. Error: {ex.Message}";
            }

            return Page();
        }
    }
}
