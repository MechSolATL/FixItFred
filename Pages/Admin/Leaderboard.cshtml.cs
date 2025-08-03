using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicianViewModel = Data.Models.UI.TechnicianViewModel;
using StatsLeaderboardService = Services.Stats.ILeaderboardService;

namespace Pages.Admin
{
    [Authorize(Roles="Admin,Manager")]
    public class LeaderboardModel : PageModel
    {
        private readonly StatsLeaderboardService _leaderboardService;
        private readonly ISeoService _seoService;
        private readonly IContentService _contentService;

        public LeaderboardModel(StatsLeaderboardService leaderboardService, ISeoService seoService, IContentService contentService)
        {
            _leaderboardService = leaderboardService;
            _seoService = seoService;
            _contentService = contentService;
        }

        public List<TechnicianViewModel> TopTechnicians { get; set; } = new();
        public string MonthlyChallengeSummary { get; set; } = string.Empty;
        public string TeamEfficiency { get; set; } = string.Empty;
        public string RotatingQuote { get; set; } = string.Empty;
        // [Sprint91_26] SEO Metadata Razor Injection Patch
        public SeoMeta Seo { get; set; } = new SeoMeta();
        public string Title => Seo.Title;
        public string MetaDescription => Seo.MetaDescription;
        public string Keywords => Seo.Keywords;
        public string Robots => Seo.Robots;

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoForPageAsync("Leaderboard");
            // Title = seo.Title; // This line is no longer needed

            TopTechnicians = await _leaderboardService.GetTopTechniciansAsync(6);
            MonthlyChallengeSummary = await _leaderboardService.GetMonthlyChallengeSummaryAsync();
            TeamEfficiency = await _leaderboardService.GetTeamEfficiencyAsync();
            RotatingQuote = await _contentService.GetContentAsync("LeaderboardQuote");
        }

        public async Task<List<TechnicianViewModel>> GetTopTechniciansAsync()
        {
            // Placeholder implementation
            return await Task.FromResult(new List<TechnicianViewModel>());
        }
    }
}
