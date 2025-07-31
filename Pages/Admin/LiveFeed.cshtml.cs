using MVP_Core.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class LiveFeedModel : PageModel
    {
        private readonly TechnicianActivityFeedService _activityService;
        public LiveFeedModel(TechnicianActivityFeedService activityService)
        {
            _activityService = activityService;
        }

        [BindProperty(SupportsGet = true)]
        public int FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterActivityType { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public int FilterTimeRange { get; set; } = 60;
        [BindProperty(SupportsGet = true)]
        public bool IsReplayMode { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ReplaySessionId { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public int ReplayStepInterval { get; set; } = 1;

        public List<TechnicianActivityFeedLog> ActivityFeed { get; set; } = new();
        public List<TechnicianActivityFeedLog> GeoTrailLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (IsReplayMode && !string.IsNullOrEmpty(ReplaySessionId))
            {
                GeoTrailLogs = await _activityService.ReplayGeoTrailAsync(ReplaySessionId, ReplayStepInterval);
                ActivityFeed = GeoTrailLogs;
            }
            else
            {
                var feed = FilterTechnicianId > 0 ? await _activityService.GetLiveFeedAsync(FilterTechnicianId) : new List<TechnicianActivityFeedLog>();
                if (!string.IsNullOrEmpty(FilterActivityType))
                    feed = feed.Where(x => x.ActivityType == FilterActivityType).ToList();
                if (FilterTimeRange > 0)
                    feed = feed.Where(x => (DateTime.UtcNow - x.Timestamp).TotalMinutes < FilterTimeRange).ToList();
                ActivityFeed = feed;
                if (!string.IsNullOrEmpty(ReplaySessionId))
                    GeoTrailLogs = await _activityService.GetGeoTrailSessionAsync(ReplaySessionId);
            }
        }
    }
}
