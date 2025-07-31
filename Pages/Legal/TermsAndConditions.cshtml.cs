using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pages.Legal
{
    public class TermsAndConditionsModel : PageModel
    {
        [BindProperty]
        public string TermsText { get; set; } = "By using Service Atlanta, you agree to the following terms and conditions.";
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
