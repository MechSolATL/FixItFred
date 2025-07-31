using Data.Models;
using System.Collections.Generic;

namespace ViewModels
{
    public class LeaderboardViewModel
    {
        public string EfficiencyRate { get; set; } = "0%";
        public string MonthlyChallenge { get; set; } = "None";
        public List<TechnicianViewModel> TopTechnicians { get; set; } = new();
        public SeoMetadata Seo { get; set; } = new SeoMetadata();
    }
}
