using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;

namespace MVP_Core.Pages.Admin
{
    public class AddTechnicianModel : PageModel
    {
        private readonly ITechnicianService _techService;

        [BindProperty]
        public Technician Technician { get; set; } = new();

        public AddTechnicianModel(ITechnicianService techService)
        {
            _techService = techService;
        }

        public void OnGet()
        {
            Technician = new Technician { IsActive = true };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _techService.AddAsync(Technician);
            return RedirectToPage("/Admin/Technicians");
        }
    }
}
