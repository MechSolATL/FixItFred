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
        public SyncIncentiveEngine SyncIncentiveEngine { get; set; }
        public List<MVP_Core.Data.Models.Technician> GetTechniciansByIds(IEnumerable<int> ids)
        {
            var db = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            return db.Technicians.Where(t => ids.Contains(t.Id)).ToList();
        }
        public void OnGet()
        {
            SyncIncentiveEngine = HttpContext.RequestServices.GetRequiredService<SyncIncentiveEngine>();
            // Refresh sync scores with new automation logic
            var scores = SyncIncentiveEngine.CalculateAllSyncScores();
            // Optionally expose to ViewData for Razor UI
            ViewData["SyncScores"] = scores;
        }
        public IActionResult OnPostOverrideRank()
        {
            SyncIncentiveEngine = HttpContext.RequestServices.GetRequiredService<SyncIncentiveEngine>();
            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("SubmitOverride"))
                {
                    var techIdStr = Request.Form[key];
                    if (int.TryParse(techIdStr, out int techId))
                    {
                        var rankKey = $"OverrideRank_{techId}";
                        var reasonKey = $"OverrideReason_{techId}";
                        if (Request.Form.ContainsKey(rankKey))
                        {
                            var rankStr = Request.Form[rankKey];
                            var reason = Request.Form[reasonKey];
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
            SyncIncentiveEngine = HttpContext.RequestServices.GetRequiredService<SyncIncentiveEngine>();
            var csv = SyncIncentiveEngine.GenerateCsvReport();
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "SyncIncentives.csv");
        }
    }
}
