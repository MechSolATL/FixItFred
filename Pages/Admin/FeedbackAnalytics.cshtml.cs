using MVP_Core.Services;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class FeedbackAnalyticsModel : PageModel
    {
        private readonly FeedbackAnalyticsService _analyticsService;
        public List<CustomerReview> Reviews { get; set; } = new();
        public List<CustomerReview> FlaggedReviews { get; set; } = new();
        public FeedbackAnalyticsModel(FeedbackAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        public void OnGet()
        {
            Reviews = _analyticsService.GetAllReviews();
            FlaggedReviews = _analyticsService.GetFlaggedReviews();
        }
    }
}
