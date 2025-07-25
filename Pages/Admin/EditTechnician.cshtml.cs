using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;

namespace MVP_Core.Pages.Admin
{
    public class EditTechnicianModel : PageModel
    {
        private readonly ITechnicianService _techService;

        [BindProperty]
        public MVP_Core.Data.Models.Technician Technician { get; set; } = new();

        public EditTechnicianModel(ITechnicianService techService)
        {
            _techService = techService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var techVm = await _techService.GetByIdAsync(id);
            if (techVm == null)
                return NotFound();
            Technician = new MVP_Core.Data.Models.Technician
            {
                Id = techVm.Id,
                FullName = techVm.FullName,
                IsActive = techVm.IsActive,
                Email = techVm.Email,
                Phone = techVm.Phone,
                Specialty = techVm.Specialty
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            await _techService.UpdateAsync(Technician);
            TempData["ToastMessage"] = "Technician updated successfully.";
            return RedirectToPage("/Admin/Technicians");
        }
    }
}
