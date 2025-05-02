// =========================
// File: Pages/Verify.cshtml.cs
// =========================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data.Models.ViewModels;

namespace MVP_Core.Pages
{
    public class VerifyModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public VerifyModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public VerificationInputModel Input { get; set; } = new();

        public string? Message { get; set; }
        public bool IsSuccess { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            var seo = await _db.SEOs.FirstOrDefaultAsync(s => s.PageName == "Verify");
            if (seo != null)
            {
                ViewData["Title"] = seo.Title;
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = "noindex, nofollow"; // optional, depends on privacy
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Invalid input.";
                return Page();
            }

            var record = await _db.EmailVerifications
                .FirstOrDefaultAsync(e => e.Email == Input.Email && !e.IsVerified);

            if (record == null)
            {
                Message = "No verification record found.";
                return Page();
            }

            if (record.Code?.Trim() != Input.Code?.Trim())
            {
                Message = "Incorrect verification code.";
                return Page();
            }

            record.IsVerified = true;
            await _db.SaveChangesAsync();

            HttpContext.Session.SetString("IsEmailVerified", "true");
            HttpContext.Session.SetString("VerifiedEmail", Input.Email);

            IsSuccess = true;
            Message = "✅ Email verified successfully.";
            return Page();
        }
    }

    public class VerificationInputModel
    {
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Code { get; set; } = string.Empty;
    }
}
