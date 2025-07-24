using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVP_Core.Services;
using MVP_Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianHeatmapModel : PageModel
    {
        private readonly ITechnicianProfileService _profileService;
        private readonly ApplicationDbContext _db;
        public TechnicianHeatmapModel(ITechnicianProfileService profileService, ApplicationDbContext db)
        {
            _profileService = profileService;
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public List<int> TechnicianIds { get; set; } = new();
        public IEnumerable<SelectListItem> Technicians { get; set; } = new List<SelectListItem>();
        [BindProperty(SupportsGet = true)]
        public DateTime Start { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime End { get; set; }

        public async Task OnGetAsync(List<int>? techIds = null, DateTime? start = null, DateTime? end = null)
        {
            ViewData["Title"] = "Technician Performance Heatmap";
            var techs = _db.Technicians.OrderBy(t => t.FullName).ToList();
            Technicians = techs.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.FullName });
            TechnicianIds = techIds ?? (techs.Count > 0 ? new List<int> { techs[0].Id } : new List<int>());
            Start = start ?? DateTime.UtcNow.AddDays(-30);
            End = end ?? DateTime.UtcNow;
            await Task.CompletedTask;
        }
    }
}
