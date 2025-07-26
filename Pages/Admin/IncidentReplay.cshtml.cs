using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class IncidentReplayModel : PageModel
    {
        private readonly TechnicianReplayService _replayService;
        private readonly ApplicationDbContext _db;
        public IncidentReplayModel(TechnicianReplayService replayService, ApplicationDbContext db)
        {
            _replayService = replayService;
            _db = db;
        }

        [BindProperty]
        public int? SelectedTechnicianId { get; set; }
        public List<TechnicianIncidentReplay> Replays { get; set; } = new();
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            if (SelectedTechnicianId.HasValue)
            {
                Replays = await _replayService.GetReplaysByTechnicianAsync(SelectedTechnicianId.Value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            if (SelectedTechnicianId.HasValue)
            {
                Replays = await _replayService.GetReplaysByTechnicianAsync(SelectedTechnicianId.Value);
            }
            return Page();
        }
    }
}
