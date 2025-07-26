using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class KarmaCenterModel : PageModel
    {
        private readonly TechnicianKarmaService _karmaService;
        private readonly ApplicationDbContext _db;
        public KarmaCenterModel(TechnicianKarmaService karmaService, ApplicationDbContext db)
        {
            _karmaService = karmaService;
            _db = db;
        }
        [BindProperty]
        public int TechnicianId { get; set; }
        public List<MVP_Core.Data.Models.Technician> TechnicianList;
        public List<TechnicianKarmaLog> KarmaHistory { get; set; } = new List<TechnicianKarmaLog>();
        public int? CurrentKarmaScore { get; set; }
        public string CurrentTrend { get; set; } = "";
        [BindProperty]
        public int ManualScore { get; set; }
        [BindProperty]
        public string AdminNote { get; set; } = "";
        public string GetKarmaColor() => GetKarmaColor(CurrentKarmaScore ?? 0);
        public string GetKarmaColor(int score)
        {
            if (score >= 80) return "green";
            if (score >= 50) return "orange";
            return "red";
        }
        public async Task OnGetAsync()
        {
            TechnicianList = await _db.Technicians.ToListAsync();
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            TechnicianList = await _db.Technicians.ToListAsync();
            if (TechnicianId > 0)
            {
                CurrentKarmaScore = await _karmaService.CalculateKarmaAsync(TechnicianId);
                KarmaHistory = await _karmaService.GetKarmaHistoryAsync(TechnicianId);
                CurrentTrend = KarmaHistory.FirstOrDefault()?.Trend ?? "Stable";
            }
            return Page();
        }
        public async Task<IActionResult> OnPostManualOverrideAsync()
        {
            TechnicianList = await _db.Technicians.ToListAsync();
            if (TechnicianId > 0)
            {
                await _karmaService.ApplyManualAdjustment(TechnicianId, ManualScore, AdminNote, User?.Identity?.Name ?? "admin");
                CurrentKarmaScore = await _karmaService.CalculateKarmaAsync(TechnicianId);
                KarmaHistory = await _karmaService.GetKarmaHistoryAsync(TechnicianId);
                CurrentTrend = KarmaHistory.FirstOrDefault()?.Trend ?? "Stable";
            }
            return Page();
        }
    }
}
