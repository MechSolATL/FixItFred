using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class SyncIncentivesModel : PageModel
    {
        public SyncIncentiveEngine SyncIncentiveEngine { get; set; } = new SyncIncentiveEngine(); // Sprint 79.2
        public List<MVP_Core.Data.Models.Technician> GetTechniciansByIds(IEnumerable<int> ids)
        {
            var db = HttpContext?.RequestServices?.GetRequiredService<ApplicationDbContext>(); // Sprint 79.2
            return db?.Technicians?.Where(t => ids.Contains(t.Id)).ToList() ?? new List<MVP_Core.Data.Models.Technician>(); // Sprint 79.2
        }
        public void OnGet()
        {
            SyncIncentiveEngine = HttpContext?.RequestServices?.GetRequiredService<SyncIncentiveEngine>() ?? new SyncIncentiveEngine(); // Sprint 79.2
            var scores = SyncIncentiveEngine.CalculateAllSyncScores(); // Sprint 79.2
            ViewData["SyncScores"] = scores;
        }
        public IActionResult OnPostOverrideRank()
        {
            SyncIncentiveEngine = HttpContext?.RequestServices?.GetRequiredService<SyncIncentiveEngine>() ?? new SyncIncentiveEngine(); // Sprint 79.2
            foreach (var key in Request?.Form?.Keys ?? new List<string>()) // Sprint 79.2
            {
                if (key.StartsWith("SubmitOverride"))
                {
                    var techIdStr = Request?.Form?[key].ToString() ?? string.Empty; // Sprint 79.2
                    if (int.TryParse(techIdStr, out int techId))
                    {
                        var rankKey = $"OverrideRank_{techId}";
                        var reasonKey = $"OverrideReason_{techId}";
                        if (Request?.Form?.ContainsKey(rankKey) ?? false) // Sprint 79.2
                        {
                            var rankStr = Request?.Form?[rankKey].ToString() ?? string.Empty; // Sprint 79.2
                            var reason = Request?.Form?[reasonKey].ToString() ?? string.Empty; // Sprint 79.2
                            if (Enum.TryParse<SyncRankLevel>(rankStr, out var newRank))
                            {
                                var user = User?.Identity?.Name ?? "admin"; // Sprint 79.2
                                SyncIncentiveEngine.OverrideSyncRank(techId, newRank, reason, user); // Sprint 79.2
                            }
                        }
                    }
                }
            }
            return RedirectToPage();
        }
        public IActionResult OnPostExportCsv()
        {
            SyncIncentiveEngine = HttpContext?.RequestServices?.GetRequiredService<SyncIncentiveEngine>() ?? new SyncIncentiveEngine(); // Sprint 79.2
            var csv = SyncIncentiveEngine.GenerateCsvReport(); // Sprint 79.2
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "SyncIncentives.csv");
        }
    }
}
