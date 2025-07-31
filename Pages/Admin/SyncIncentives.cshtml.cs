using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Data;
using Services.Admin;
using Data.Models;

namespace Pages.Admin
{
    public class SyncIncentivesModel : PageModel
    {
        private readonly ApplicationDbContext _db; // Sprint 79.5: Injected _db for SyncIncentiveEngine
        public SyncIncentiveEngine SyncIncentiveEngine { get; set; }
        public SyncIncentivesModel(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint 79.5: Injected _db for SyncIncentiveEngine
            SyncIncentiveEngine = new SyncIncentiveEngine(_db); // Sprint 79.5: Injected _db for SyncIncentiveEngine
        }
        public List<Data.Models.Technician> GetTechniciansByIds(IEnumerable<int> ids)
        {
            return _db?.Technicians?.Where(t => ids.Contains(t.Id)).ToList() ?? new List<Data.Models.Technician>(); // Sprint 79.5: Injected _db for SyncIncentiveEngine
        }
        public void OnGet()
        {
            SyncIncentiveEngine = new SyncIncentiveEngine(_db); // Sprint 79.5: Injected _db for SyncIncentiveEngine
            var scores = SyncIncentiveEngine.CalculateAllSyncScores();
            ViewData["SyncScores"] = scores;
        }
        public IActionResult OnPostOverrideRank()
        {
            SyncIncentiveEngine = new SyncIncentiveEngine(_db); // Sprint 79.5: Injected _db for SyncIncentiveEngine
            foreach (var key in Request?.Form?.Keys ?? new List<string>())
            {
                if (key.StartsWith("SubmitOverride"))
                {
                    var techIdStr = Request?.Form?[key].ToString() ?? string.Empty;
                    if (int.TryParse(techIdStr, out int techId))
                    {
                        var rankKey = $"OverrideRank_{techId}";
                        var reasonKey = $"OverrideReason_{techId}";
                        if (Request?.Form?.ContainsKey(rankKey) ?? false)
                        {
                            var rankStr = Request?.Form?[rankKey].ToString() ?? string.Empty;
                            var reason = Request?.Form?[reasonKey].ToString() ?? string.Empty;
                            if (Enum.TryParse<SyncRankLevel>(rankStr, out var newRank))
                            {
                                var user = User?.Identity?.Name ?? "admin";
                                SyncIncentiveEngine.OverrideSyncRank(techId, newRank, reason, user);
                            }
                        }
                    }
                }
            }
            return RedirectToPage();
        }
        public IActionResult OnPostExportCsv()
        {
            SyncIncentiveEngine = new SyncIncentiveEngine(_db); // Sprint 79.5: Injected _db for SyncIncentiveEngine
            var csv = SyncIncentiveEngine.GenerateCsvReport();
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "SyncIncentives.csv");
        }
    }
}
