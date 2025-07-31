using System.Security.Claims;
using Data;
using Data.Models;
using Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services.Admin;

namespace Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly TrustScoreDropAlertService _dropAlertService; // Sprint 84.9

        public LoginModel(ApplicationDbContext context, TrustScoreDropAlertService dropAlertService)
        {
            _context = context;
            _dropAlertService = dropAlertService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            ViewData["Title"] = "Admin Login";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill out both fields.";
                return Page();
            }

            AdminUser? adminUser = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == Username);

            if (adminUser == null || !PasswordHasher.VerifyHashedPassword(adminUser.PasswordHash, Password))
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            // Sprint 84.9 — Drop Alert Logic + TrustScore Delta Detection
            await _dropAlertService.CheckAndLogDropsAsync();

            List<Claim> claims =
            [
                new Claim(ClaimTypes.Name, adminUser.Username),
                new Claim(ClaimTypes.Role, adminUser.Role ?? "Admin")
            ];

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return adminUser.LastProfileReviewDate == null || adminUser.LastProfileReviewDate <= DateTime.UtcNow.AddDays(-30)
                ? RedirectToPage("/Admin/ProfileReview")
                : RedirectToPage("/Admin/Index");
        }
    }
}
