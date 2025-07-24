using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVP_Core.Pages.Admin.BlazorAdmin.Components
{
    public class EscalationModalModel : PageModel
    {
        [BindProperty]
        public string JobId { get; set; } = string.Empty;
        [BindProperty]
        public string EscalationNote { get; set; } = string.Empty;

        public void OnGet() { }
        public IActionResult OnPost()
        {
            // Handle escalation logic here, e.g., call a service or raise an event
            // Optionally close modal via JS on success
            return Page();
        }
    }
}
