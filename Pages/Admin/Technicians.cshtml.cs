using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pages.Admin
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
