using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MVP_Core.Pages.Admin
{
    public class WarningCenterModel : PageModel
    {
        private readonly TechnicianWarningService _warningService;
        private readonly ApplicationDbContext _db;
        public WarningCenterModel(TechnicianWarningService warningService, ApplicationDbContext db)
        {
            _warningService = warningService;
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int? TechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Zone { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Severity { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FromDateString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ToDateString { get; set; }
        public DateTime? FromDate => DateTime.TryParse(FromDateString, out var dt) ? dt : null;
        public DateTime? ToDate => DateTime.TryParse(ToDateString, out var dt) ? dt : null;

        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new List<MVP_Core.Data.Models.Technician>();
        public List<TechnicianWarningLog> WarningLogs { get; set; } = new List<TechnicianWarningLog>();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.Where(t => t.IsActive).OrderBy(t => t.FullName).ToList();
            WarningLogs = await _warningService.GetWarningsAsync(TechnicianId, FromDate, ToDate, Zone, Severity);
        }

        public async Task<IActionResult> OnPostResolveAsync(int id)
        {
            await _warningService.ResolveWarningAsync(id);
            return RedirectToPage(new { TechnicianId, Zone, Severity, FromDateString, ToDateString });
        }
    }
}
