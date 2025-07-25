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
        public int TechId { get; private set; }
        public MVP_Core.Models.Admin.TechnicianProfileDto? Profile { get; private set; }
        public double AvgFeedback { get; private set; }
        public List<TechnicianFeedback> RecentReviews { get; private set; } = new();

        public TechnicianProfileModel(DispatcherService dispatcherService, TechnicianFeedbackService feedbackService)
        {
            _dispatcherService = dispatcherService;
            _feedbackService = feedbackService;
        }

        public void OnGet()
        {
            TechId = int.TryParse(Request.Query["techId"], out var tId) ? tId : 0;
            // FixItFred — Sprint 44.4 Await Fix (GetTechnicianProfile)
            Profile = _dispatcherService.GetTechnicianProfile(TechId).GetAwaiter().GetResult();
            AvgFeedback = _feedbackService.CalculateAverageRating(TechId);
            RecentReviews = _feedbackService.GetFeedbackForTechnician(TechId).Take(3).ToList();
        }
    }
}
