// [FixItFred] Patch Log: Forced explicit use of MVP_Core.Models.Admin.TechnicianProfileDto for all DTO references
using MVP_Core.Models.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianProfileModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        private readonly TechnicianFeedbackService _feedbackService;
        private readonly TechnicianBonusService _bonusService;
        public int TechId { get; private set; }
        public MVP_Core.Models.Admin.TechnicianProfileDto? Profile { get; private set; }
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
