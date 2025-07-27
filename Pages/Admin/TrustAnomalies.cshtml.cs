using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class TrustAnomaliesModel : PageModel
    {
        private readonly TrustGraphAnalyzerService _anomalyService;
        public TrustAnomaliesModel(TrustGraphAnalyzerService anomalyService)
        {
            _anomalyService = anomalyService;
        }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = string.Empty;
        public List<TrustAnomalyLog> AnomalyLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            var logs = await _anomalyService.AnalyzeTrustGraphAsync();
            if (!string.IsNullOrEmpty(StatusFilter))
                logs = logs.Where(x => x.Status == StatusFilter).ToList();
            AnomalyLogs = logs;
        }

        public async Task<IActionResult> OnPostAsync(int AnomalyId, string ReviewedBy)
        {
            var log = (await _anomalyService.DetectAnomaliesAsync()).FirstOrDefault(x => x.Id == AnomalyId);
            if (log != null)
            {
                log.Status = "Reviewed";
                log.ReviewedBy = ReviewedBy;
                await _anomalyService.LogAnomalyAsync(log.TechnicianId, log.AnomalyType, log.GraphNodeContext, log.AnomalyScore, ReviewedBy, "Reviewed");
            }
            return RedirectToPage();
        }
    }
}
