// [FixItFred] Patch Log: Forced explicit use of Models.Admin.TechnicianProfileDto for all DTO references
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Services;
using Services.Admin;

namespace Pages.Admin
{
    public class TechnicianProfileModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        private readonly TechnicianFeedbackService _feedbackService;
        private readonly TechnicianBonusService _bonusService;
        public int TechId { get; private set; }
        public Models.Admin.TechnicianProfileDto? Profile { get; private set; }
        public double AvgFeedback { get; private set; }
        public List<TechnicianFeedback> RecentReviews { get; private set; } = new();
        public List<SkillBadge> SkillBadges { get; set; } = new();
        public List<TechnicianBonusLog> BonusHistory { get; set; } = new();

        public TechnicianProfileModel(DispatcherService dispatcherService, TechnicianFeedbackService feedbackService, TechnicianBonusService bonusService)
        {
            _dispatcherService = dispatcherService;
            _feedbackService = feedbackService;
            _bonusService = bonusService;
        }

        public void OnGet()
        {
            TechId = int.TryParse(Request.Query["techId"], out var tId) ? tId : 0;
            Profile = _dispatcherService.GetTechnicianProfile(TechId).GetAwaiter().GetResult();
            AvgFeedback = _feedbackService.CalculateAverageRating(TechId);
            RecentReviews = _feedbackService.GetFeedbackForTechnician(TechId).Take(3).ToList();
            SkillBadges = new List<SkillBadge>(); // TODO: Load from DB if needed
            BonusHistory = _bonusService.GetBonuses(TechId);
        }
    }
}
