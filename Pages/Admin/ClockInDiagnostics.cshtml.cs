using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class ClockInDiagnosticsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<LateClockInLog> ClockInLogs { get; set; } = new();
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();
        public HashSet<int> EscalatedTechIds { get; set; } = new();

        public ClockInDiagnosticsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            ClockInLogs = _db.LateClockInLogs.OrderByDescending(l => l.Date).Take(100).ToList();
            Technicians = _db.Technicians.ToList();
            var sevenDaysAgo = System.DateTime.UtcNow.AddDays(-7);
            EscalatedTechIds = _db.EscalationEvents
                .Where(e => e.Status == "Open" && e.IncidentDate >= sevenDaysAgo)
                .Select(e => e.TechnicianId)
                .ToHashSet();
        }
    }
}
