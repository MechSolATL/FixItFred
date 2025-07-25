using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services; // Sprint 46.2 – Customer Ticket Analytics Backend
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Customer
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly CustomerTicketAnalyticsService _analyticsService; // Sprint 46.2 – Customer Ticket Analytics Backend
        public DashboardModel(ApplicationDbContext db)
        {
            _db = db;
            _analyticsService = new CustomerTicketAnalyticsService(db); // Sprint 46.2 – Customer Ticket Analytics Backend
        }

        public List<ServiceRequest> Tickets { get; set; } = new();
        public double AverageSatisfaction { get; set; }
        // Sprint 46.2 – Customer Ticket Analytics Backend
        public int TicketCount { get; set; }
        public double AverageResponseTime { get; set; }
        public int[] SatisfactionTrend { get; set; } = new int[0];

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
