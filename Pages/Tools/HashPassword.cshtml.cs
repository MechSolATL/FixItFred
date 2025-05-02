using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Helpers; // ✅ Import your PasswordHasher

namespace MVP_Core.Pages.Tools
{
    public class HashPasswordModel : PageModel
    {
        [BindProperty]
        public string InputPassword { get; set; } = string.Empty;

        public string? GeneratedHash { get; set; }

        public void OnGet()
        {
            // Optional: Preload defaults if needed
        }

        public void OnPost()
        {
            if (!string.IsNullOrWhiteSpace(InputPassword))
            {
                GeneratedHash = PasswordHasher.HashPassword(InputPassword);
            }
        }
    }
}
