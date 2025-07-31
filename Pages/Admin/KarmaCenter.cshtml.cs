using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class KarmaCenterModel : PageModel
    {
        private readonly TechnicianKarmaService _karmaService; // Sprint 79.3: CS8618/CS860X warning cleanup
        private readonly ApplicationDbContext _db; // Sprint 79.3: CS8618/CS860X warning cleanup
        public KarmaCenterModel(TechnicianKarmaService karmaService, ApplicationDbContext db)
        {
            _karmaService = karmaService ?? throw new ArgumentNullException(nameof(karmaService)); // Sprint 79.3: CS8618/CS860X warning cleanup
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint 79.3: CS8618/CS860X warning cleanup
            TechnicianList = new List<Data.Models.Technician>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            KarmaHistory = new List<TechnicianKarmaLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            CurrentTrend = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
            AdminNote = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        }
        [BindProperty]
        public int TechnicianId { get; set; } // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<Data.Models.Technician> TechnicianList { get; set; } = new List<Data.Models.Technician>(); // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<TechnicianKarmaLog> KarmaHistory { get; set; } = new List<TechnicianKarmaLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
        public int? CurrentKarmaScore { get; set; } // Sprint 79.3: CS8618/CS860X warning cleanup
        public string CurrentTrend { get; set; } = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public int ManualScore { get; set; } // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public string AdminNote { get; set; } = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        public string GetKarmaColor() => GetKarmaColor(CurrentKarmaScore ?? 0); // Sprint 79.3: CS8618/CS860X warning cleanup
        public string GetKarmaColor(int score)
        {
            if (score >= 80) return "green";
            if (score >= 50) return "orange";
            return "red";
        }
        public async Task OnGetAsync()
        {
            TechnicianList = await _db.Technicians.ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
        }
        public async Task<IActionResult> OnPostFilterAsync()
        {
            TechnicianList = await _db.Technicians.ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
            if (TechnicianId > 0)
            {
                CurrentKarmaScore = await _karmaService.CalculateKarmaAsync(TechnicianId); // Sprint 79.3: CS8618/CS860X warning cleanup
                KarmaHistory = await _karmaService.GetKarmaHistoryAsync(TechnicianId); // Sprint 79.3: CS8618/CS860X warning cleanup
                CurrentTrend = KarmaHistory.FirstOrDefault()?.Trend ?? "Stable"; // Sprint 79.3: CS8618/CS860X warning cleanup
            }
            return Page();
        }
        public async Task<IActionResult> OnPostManualOverrideAsync()
        {
            TechnicianList = await _db.Technicians.ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
            if (TechnicianId > 0)
            {
                await _karmaService.ApplyManualAdjustment(TechnicianId, ManualScore, AdminNote, User?.Identity?.Name ?? "admin"); // Sprint 79.3: CS8618/CS860X warning cleanup
                CurrentKarmaScore = await _karmaService.CalculateKarmaAsync(TechnicianId); // Sprint 79.3: CS8618/CS860X warning cleanup
                KarmaHistory = await _karmaService.GetKarmaHistoryAsync(TechnicianId); // Sprint 79.3: CS8618/CS860X warning cleanup
                CurrentTrend = KarmaHistory.FirstOrDefault()?.Trend ?? "Stable"; // Sprint 79.3: CS8618/CS860X warning cleanup
            }
            return Page();
        }
    }
}
