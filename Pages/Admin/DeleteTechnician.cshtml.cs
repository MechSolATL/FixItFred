using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;

namespace MVP_Core.Pages.Admin
{
    public class DeleteTechnicianModel : PageModel
    {
        private readonly ITechnicianService _techService;

        [BindProperty]
        public Technician? Technician { get; set; }

        public DeleteTechnicianModel(ITechnicianService techService)
        {
            _techService = techService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var techVm = await _techService.GetByIdAsync(id);
            if (techVm == null)
                return NotFound();
            Technician = new Technician
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _techService.DeleteAsync(id);
            TempData["ToastMessage"] = "Technician removed";
            return RedirectToPage("/Admin/Technicians");
        }
    }
}
