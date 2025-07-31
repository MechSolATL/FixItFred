// 🛠 Sprint 91.9-B: AdminLogin SEO and Layout Compliance Patch
// ✨ Injects full SEO metadata dynamically
// ✨ Declares Layout explicitly for Razor compliance
// 🧠 Provides strong comments for all functional blocks

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Helpers;
using Data.Models;
using Data;
using Services;

namespace Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISeoService _seoService;

        public AdminLoginModel(ApplicationDbContext context, ISeoService seoService)
        {
            _context = context;
            _seoService = seoService;
        }

        // 🧾 Form fields with required validation
        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        // ❗ Used for feedback messaging
        public string ErrorMessage { get; set; } = string.Empty;

        // 🔍 Loaded SEO metadata
        public SeoMetadata? Seo { get; set; }

        public async Task OnGetAsync()
        {
            // 🎯 Set Razor compliance ViewData + Layout
            ViewData["Title"] = "Admin Login";
            Layout = "/Pages/Shared/_Layout.cshtml";

            // 🧠 Inject SEO metadata dynamically
            Seo = await _seoService.GetSeoByPageNameAsync("AdminLogin");
            if (Seo != null)
            {
                ViewData["Title"] = Seo.Title;
                ViewData["MetaDescription"] = Seo.MetaDescription;
                ViewData["Keywords"] = Seo.Keywords;
                ViewData["Robots"] = Seo.Robots;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please complete all required fields.";
                return Page();
            }

            AdminUser? adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.Username == Username);

            if (adminUser == null ||
                !PasswordHasher.VerifyHashedPassword(adminUser.PasswordHash, Password))
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, adminUser.Username),
                new Claim(ClaimTypes.Role, adminUser.Role ?? "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // 🔄 Check profile freshness for redirect logic
            return adminUser.LastProfileReviewDate == null || adminUser.LastProfileReviewDate <= DateTime.UtcNow.AddDays(-30)
                ? RedirectToPage("/Admin/ProfileReview")
                : RedirectToPage("/Admin/Index");
        }
    }
}
