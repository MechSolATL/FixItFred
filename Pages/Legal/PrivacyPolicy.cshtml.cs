using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVP_Core.Pages.Legal
{
    public class PrivacyPolicyModel : PageModel
    {
        [BindProperty]
        public string PolicyText { get; set; } = "Your privacy is important to us. This policy explains how Service Atlanta collects, uses, and protects your information.";
        public void OnGet()
        {
            // Optionally load from DB or file
        }
        public void OnPost()
        {
            // Optionally save to DB or file
        }
    }
}
