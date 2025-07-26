using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class TechForecastModel : PageModel
    {
        private readonly TechnicianForecastService _forecastService;
        private readonly ApplicationDbContext _db;

        public TechForecastModel(TechnicianForecastService forecastService, ApplicationDbContext db)
        {
            _forecastService = forecastService;
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? TechnicianId { get; set; }

        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new List<MVP_Core.Data.Models.Technician>();
        public List<TechnicianForecastLog> Forecasts { get; set; } = new List<TechnicianForecastLog>();

        public async Task OnGetAsync()
        {
            Technicians = await _db.Technicians.ToListAsync();
            Forecasts = await _db.TechnicianForecastLogs
                .Where(f => (!StartDate.HasValue || f.ForecastDate >= StartDate.Value)
                         && (!EndDate.HasValue || f.ForecastDate <= EndDate.Value)
                         && (!TechnicianId.HasValue || f.TechnicianId == TechnicianId.Value))
                .OrderByDescending(f => f.ForecastDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostRunForecastAsync()
        {
            await _forecastService.GenerateForecastsAsync(StartDate, EndDate, TechnicianId);
            return RedirectToPage();
        }
    }
}
