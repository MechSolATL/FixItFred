using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System;
using Services;
using Data.Models;

namespace Pages.Technician
{
    // Sprint 84.7.2 — Live Filter + UI Overlay
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Technician")]
    public class ReviewsModel : PageModel
    {
        private readonly ITechnicianService _technicianService;
        private readonly IReviewService _reviewService;
        private readonly ITechnicianLeaderboardService _leaderboardService;

        public ReviewsModel(ITechnicianService technicianService, IReviewService reviewService, ITechnicianLeaderboardService leaderboardService)
        {
            _technicianService = technicianService;
            _reviewService = reviewService;
            _leaderboardService = leaderboardService;
        }

        public int TechnicianId { get; set; }
        public List<RewardTier> Tiers { get; set; } = new();
        public string SelectedTier { get; set; } = string.Empty;
        public List<CustomerReview> FilteredReviews { get; set; } = new();
        public ReviewStats Stats { get; set; } = new();
        public int[] HeatmapRatings { get; set; } = new int[5];
        public Data.Models.Technician TechnicianDetail { get; set; } // Sprint 84.7.2 — Live Filter + UI Overlay
        public int? SelectedReviewId { get; set; } // Sprint 84.7.2 — Live Filter + UI Overlay

        public void OnGet(int? technicianId, string? tier, int? reviewId)
        {
            TechnicianId = technicianId ?? _technicianService.GetCurrentTechnicianId(User);
            Tiers = _leaderboardService.GetTechnicianTiers();
            SelectedTier = tier ?? string.Empty;
            var allReviews = _reviewService.GetApprovedReviewsByTechnician(TechnicianId);
            if (!string.IsNullOrEmpty(SelectedTier))
            {
                FilteredReviews = allReviews.Where(r => r.Tier == SelectedTier).ToList();
            }
            else
            {
                FilteredReviews = allReviews;
            }
            Stats = _leaderboardService.GetReviewStats(TechnicianId, SelectedTier);
            HeatmapRatings = new int[5];
            foreach (var r in FilteredReviews)
            {
                if (r.Rating >= 1 && r.Rating <= 5)
                    HeatmapRatings[r.Rating - 1]++;
            }
            TechnicianDetail = _technicianService.GetTechnicianById(TechnicianId); // Sprint 84.7.2 — Live Filter + UI Overlay
            SelectedReviewId = reviewId;
        }

        public PartialViewResult OnGetFilter(string tier, int? technicianId)
        {
            TechnicianId = technicianId ?? _technicianService.GetCurrentTechnicianId(User);
            var allReviews = _reviewService.GetApprovedReviewsByTechnician(TechnicianId);
            var filtered = string.IsNullOrEmpty(tier) ? allReviews : allReviews.Where(r => r.Tier == tier).ToList();
            FilteredReviews = filtered;
            return Partial("_ReviewsTable", this);
        }

        public class ReviewStats
        {
            public string TierName { get; set; } = string.Empty;
            public string BadgeColor { get; set; } = "#2196f3";
            public double AverageRating { get; set; }
            public int TotalReviews { get; set; }
        }
    }
}
