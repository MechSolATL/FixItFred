// FixItFred Patch Log — Sprint 27
// [2024-07-25T00:00:00Z] — TechnicianMapModel scaffolded for map view. Loads technician geolocation/status for Razor page.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using MVP_Core.Services.Admin;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianMapModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public TechnicianMapModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        public List<TechnicianStatusDto> Technicians { get; private set; } = new();
        public void OnGet()
        {
            Technicians = _dispatcherService.GetAllTechnicianStatuses();
            // Demo: inject mock locations if missing
            double[] lats = { 33.7510, 33.7550, 33.7495, 33.7480, 33.7525 };
            double[] lngs = { -84.3900, -84.3850, -84.3885, -84.3920, -84.3875 };
            for (int i = 0; i < Technicians.Count; i++)
            {
                Technicians[i].Latitude ??= lats[i % lats.Length];
                Technicians[i].Longitude ??= lngs[i % lngs.Length];
            }
        }
    }
}
