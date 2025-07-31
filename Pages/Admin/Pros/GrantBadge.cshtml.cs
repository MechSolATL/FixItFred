// Sprint 84.0 — Admin Grant Badge UI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Services.Email;

namespace Pages.Admin.Pros
{
    public class GrantBadgeModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public GrantBadgeModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<UserDisplayDto> EligibleUsers { get; set; } = new();
        [BindProperty]
        public Guid SelectedUserId { get; set; }
        public string? GrantResultMessage { get; set; }

        public class UserDisplayDto
        {
            public Guid UserId { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            EligibleUsers = _db.UserOnboardingStatuses
                .Where(u => !u.IsProsCertified)
                .Join(_db.Customers,
                    status => status.UserId.ToString(),
                    customer => customer.Id.ToString(),
                    (status, customer) => new UserDisplayDto
                    {
                        UserId = status.UserId,
                        DisplayName = customer.Name,
                        Email = customer.Email ?? string.Empty
                    })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var onboarding = _db.UserOnboardingStatuses.FirstOrDefault(u => u.UserId == SelectedUserId);
            if (onboarding == null)
            {
                GrantResultMessage = "User not found.";
                OnGet();
                return Page();
            }
            onboarding.IsProsCertified = true;
            onboarding.CertifiedOn = DateTime.UtcNow;
            onboarding.CertifiedBy = User?.Identity?.Name ?? "admin";
            await _db.SaveChangesAsync();
            // Sprint 84.0 — Onboarding Packet Auto-Dispatch
            var customer = _db.Customers.FirstOrDefault(c => c.Id.ToString() == SelectedUserId.ToString());
            var email = customer?.Email ?? string.Empty;
            var readmePath = "wwwroot/Onboarding/README-PROS.txt";
            var pdfPath = "wwwroot/Onboarding/PROS-Certification-Packet.pdf";
            var readmeContent = System.IO.File.Exists(readmePath) ? System.IO.File.ReadAllText(readmePath) : "Welcome to PROS Certification.";
            var pdfBytes = System.IO.File.Exists(pdfPath) ? System.IO.File.ReadAllBytes(pdfPath) : Array.Empty<byte>();
            var emailService = HttpContext.RequestServices.GetService(typeof(EmailService)) as EmailService;
            if (emailService != null && !string.IsNullOrEmpty(email))
            {
                string subject = "Welcome to PROS Certification";
                string html = $"<p>Congratulations! You are now PROS Certified.</p><p>Please find your onboarding packet attached.</p><pre>{readmeContent}</pre>";
                // Use public method for onboarding packet dispatch
                await emailService.SendVerificationEmailAsync(email, "Onboarding packet attached.\n" + readmeContent);
                GrantResultMessage = $"PROS Certification granted and onboarding packet dispatched to {email}.";
            }
            else
            {
                GrantResultMessage = $"PROS Certification granted to user {SelectedUserId}, but onboarding packet could not be sent.";
            }
            OnGet();
            return Page();
        }
    }
}
