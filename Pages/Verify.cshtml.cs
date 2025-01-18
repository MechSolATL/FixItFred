using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages
{
    public class VerifyModel : PageModel
    {
        [BindProperty]
        public string Message { get; set; } = string.Empty;  // ✅ Initialized

        public IActionResult OnGet(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                Message = "Invalid verification link.";
                return Page();
            }

            // Verification logic here...

            return Page();
        }
    }
}
