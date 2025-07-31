// Sprint 26.5 Patch Log: CS860x/CS8625/CS1998/CS0219 fixes — Nullability, async, and unused variable corrections for Nova review
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Data;
using Data.Models;
using Services;

namespace Pages.Customer
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly CustomerTicketAnalyticsService _analyticsService; // Sprint 46.2 – Customer Ticket Analytics Backend
        private readonly CustomerPortalService _portalService;
        public DashboardModel(ApplicationDbContext db)
        {
            _db = db;
            _analyticsService = new CustomerTicketAnalyticsService(db); // Sprint 46.2 – Customer Ticket Analytics Backend
            _portalService = new CustomerPortalService(db);
        }

        public List<ServiceRequest> Tickets { get; set; } = new();
        public double AverageSatisfaction { get; set; }
        // Sprint 46.2 – Customer Ticket Analytics Backend
        public int TicketCount { get; set; }
        public double AverageResponseTime { get; set; }
        public int[] SatisfactionTrend { get; set; } = new int[0];
        // Sprint 46.3 – Dashboard Analytics
        public Dictionary<string, int> ServiceTypeCounts { get; set; } = new();
        public int OpenCount { get; set; }
        public int ClosedCount { get; set; }
        public double AvgResolutionHours { get; set; }
        public int[] SatisfactionHeatmap { get; set; } = new int[0];
        // Sprint 84.3: Add properties for reward automation and UI feedback
        public List<LoyaltyPointTransaction> Loyalty { get; set; } = new();
        public List<RewardTier> Tiers { get; set; } = new();
        public RewardTier? CurrentTier { get; set; }
        public List<RewardTier> UnlockedTiers { get; set; } = new();

        public void OnGet()
        {
            // TODO: Replace with actual customer identity/session
            string customerEmail = User.Identity?.Name ?? "demo@customer.com";
            Tickets = _db.ServiceRequests.Where(r => r.Email == customerEmail).OrderByDescending(r => r.RequestedAt).ToList();
            if (Tickets.Any(t => t.SatisfactionScore.HasValue))
                AverageSatisfaction = Tickets.Where(t => t.SatisfactionScore.HasValue).Average(t => t.SatisfactionScore.Value);
            else
                AverageSatisfaction = 0;

            // Sprint 46.2 – Customer Ticket Analytics Backend
            var customer = _db.Customers.FirstOrDefault(c => c.Email == customerEmail);
            if (customer != null)
            {
                TicketCount = _analyticsService.GetTicketCountForCustomer(customer.Id);
                AverageResponseTime = _analyticsService.GetAverageResponseTime(customer.Id);
                SatisfactionTrend = _analyticsService.GetSatisfactionRatingTrend(customer.Id);
            }
            else
            {
                TicketCount = 0;
                AverageResponseTime = 0;
                SatisfactionTrend = new int[0];
            }

            // Sprint 46.3 – Dashboard Analytics: Service Type Breakdown
            ServiceTypeCounts = Tickets.GroupBy(t => t.ServiceType)
                .ToDictionary(g => g.Key, g => g.Count());

            // Sprint 46.3 – Dashboard Analytics: Ticket Resolution Chart
            OpenCount = Tickets.Count(t => t.Status != "Complete" && t.Status != "Cancelled");
            ClosedCount = Tickets.Count(t => t.Status == "Complete" || t.Status == "Cancelled");
            var closedTickets = Tickets.Where(t => t.ClosedAt.HasValue).ToList(); // Fix: Use .HasValue for nullable DateTime
            if (closedTickets.Any())
                AvgResolutionHours = closedTickets.Average(t => (t.ClosedAt!.Value - t.RequestedAt).TotalHours);
            else
                AvgResolutionHours = 0;

            // Sprint 46.3 – Dashboard Analytics: Satisfaction Heatmap
            SatisfactionHeatmap = Tickets.OrderByDescending(t => t.RequestedAt).Take(30).Select(t => t.SatisfactionScore ?? 0).ToArray();

            // Sprint 84.3: Reward automation and UI feedback
            Loyalty = _portalService.GetLoyaltyTransactions(customerEmail);
            Tiers = _portalService.GetRewardTiers();
            CurrentTier = Tiers.LastOrDefault(t => Loyalty.Sum(l => l.Points) >= t.PointsRequired);
            UnlockedTiers = Tiers.Where(t => Loyalty.Sum(l => l.Points) >= t.PointsRequired).ToList();
        }

        public string GetStatusClass(string status)
        {
            return status switch
            {
                "Open" => "primary",
                "Assigned" => "info",
                "Delayed" => "warning",
                "Complete" => "success",
                "Cancelled" => "secondary",
                _ => "light"
            };
        }
    }
}
