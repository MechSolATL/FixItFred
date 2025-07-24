using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class TechniciansModel : PageModel
    {
        private readonly ITechnicianService _techService;

        public List<TechnicianViewModel> Technicians { get; set; } = new();

        public TechniciansModel(ITechnicianService techService)
        {
            _techService = techService;
        }

        public async Task OnGetAsync()
        {
            Technicians = await _techService.GetAllAsync();
        }
    }
}
