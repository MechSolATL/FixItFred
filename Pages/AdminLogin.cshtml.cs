using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MVP_Core.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminLoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Ensures validation in UI and avoids CS8618
        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        // ✅ Warning-free property
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // No logic on GET
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please complete all required fields.";
                return Page();
            }

            var adminUser = await _context.AdminUsers
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

            return (adminUser.LastProfileReviewDate == null || adminUser.LastProfileReviewDate <= DateTime.UtcNow.AddDays(-30))
                ? RedirectToPage("/Admin/ProfileReview")
                : RedirectToPage("/Admin/Index");
        }
    }
}
