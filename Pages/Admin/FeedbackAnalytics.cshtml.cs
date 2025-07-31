using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Services;

namespace Pages.Admin
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
