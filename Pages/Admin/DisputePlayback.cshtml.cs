using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using MVP_Core.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class DisputePlaybackModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly DisputeInsightService _insightService;
        private readonly SessionPlaybackService _playbackService;
        public List<DisputeRecord> Disputes { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? SelectedDisputeId { get; set; }
        public List<SessionPlaybackLog> SessionLogs { get; set; } = new();
        public List<DisputeInsightLog> InsightLogs { get; set; } = new();
        public DisputePlaybackModel(ApplicationDbContext db, DisputeInsightService insightService, SessionPlaybackService playbackService)
        {
            _db = db;
            _insightService = insightService;
            _playbackService = playbackService;
        }
        public async Task OnGetAsync()
        {
            Disputes = _db.DisputeRecords.OrderByDescending(x => x.SubmittedDate).Take(50).ToList();
            if (SelectedDisputeId != null)
            {
                var dispute = _db.DisputeRecords.FirstOrDefault(x => x.Id == SelectedDisputeId);
                if (dispute != null)
                {
                    SessionLogs = await _playbackService.GetSessionEventsAsync(dispute.ServiceRequestId.ToString());
                    InsightLogs = await _insightService.GetInsightsForDisputeAsync(dispute.Id);
                }
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Disputes = _db.DisputeRecords.OrderByDescending(x => x.SubmittedDate).Take(50).ToList();
            if (SelectedDisputeId != null)
            {
                var dispute = _db.DisputeRecords.FirstOrDefault(x => x.Id == SelectedDisputeId);
                if (dispute != null)
                {
                    var type = Request.Form["InsightType"];
                    var desc = Request.Form["Description"];
                    var user = Request.Form["LoggedBy"];
                    await _insightService.LogInsightAsync(dispute.Id, type, desc, user);
                    SessionLogs = await _playbackService.GetSessionEventsAsync(dispute.ServiceRequestId.ToString());
                    InsightLogs = await _insightService.GetInsightsForDisputeAsync(dispute.Id);
                }
            }
            return Page();
        }
    }
}
