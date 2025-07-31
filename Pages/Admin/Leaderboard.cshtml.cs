using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models.UI;
using MVP_Core.Services.Stats;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles="Admin,Manager")]
    public class LeaderboardModel : PageModel
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly ContentService _contentService;
        private readonly ISeoService _seoService;

        public LeaderboardModel(ILeaderboardService leaderboardService, ContentService contentService, ISeoService seoService)
        {
            _leaderboardService = leaderboardService;
            _contentService = contentService;
            _seoService = seoService;
        }

        public string Title { get; set; }
        public List<TechnicianViewModel> TopTechnicians { get; set; } = new();
        public string MonthlyChallengeSummary { get; set; } = string.Empty;
        public int TeamEfficiency { get; set; } = 0;
        public string RotatingQuote { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoForPageAsync("Leaderboard");
            Title = seo.Title;

            TopTechnicians = await _leaderboardService.GetTopTechniciansAsync(6);
            MonthlyChallengeSummary = await _leaderboardService.GetMonthlyChallengeSummaryAsync();
            TeamEfficiency = await _leaderboardService.GetTeamEfficiencyAsync();
            RotatingQuote = await _contentService.GetContentAsync("LeaderboardQuote");
        }
    }
}
