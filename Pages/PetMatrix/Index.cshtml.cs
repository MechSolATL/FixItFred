using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Pages.PetMatrix
{
    /// <summary>
    /// Page model for the PetMatrix Protocol main interface.
    /// Displays pets, snack shop, aura status, and interaction controls.
    /// </summary>
    public class PetMatrixModel : PageModel
    {
        private readonly ILogger<PetMatrixModel> _logger;

        public PetMatrixModel(ILogger<PetMatrixModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("[Sprint135_PetMatrix] PetMatrix Protocol page accessed");
            
            // Page initialization logic would go here
            // For now, the JavaScript handles all the API calls
            
            await Task.CompletedTask;
        }
    }
}