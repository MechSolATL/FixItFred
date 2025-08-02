using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Pages.Services.ServiceRequest
{
    /// <summary>
    /// [Sprint123_FixItFred] Created missing StartModel to resolve namespace compilation errors
    /// Represents the page model for starting a service request.
    /// </summary>
    public class StartModel : PageModel
    {
        /// <summary>
        /// The type of service being requested.
        /// </summary>
        [BindProperty]
        [Required(ErrorMessage = "Please select a service type.")]
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// Customer contact information.
        /// </summary>
        [BindProperty]
        [Required(ErrorMessage = "Please provide your contact information.")]
        public string CustomerContact { get; set; } = string.Empty;

        /// <summary>
        /// Handles GET requests to display the service request start form.
        /// </summary>
        public void OnGet()
        {
            // Initialize page for service request start
        }

        /// <summary>
        /// Handles POST requests to process the service request start form.
        /// </summary>
        /// <returns>Redirect to next step or stays on page if validation fails.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Process service request initialization
            // For now, redirect to a confirmation page or next step
            return RedirectToPage("/Services/ThankYou");
        }
    }
}