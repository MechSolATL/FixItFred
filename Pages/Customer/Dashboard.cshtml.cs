using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Customer
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DashboardModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ServiceRequest> Tickets { get; set; } = new();
        public double AverageSatisfaction { get; set; }

        public void OnGet()
        {
            // TODO: Replace with actual customer identity/session
            string customerEmail = User.Identity?.Name ?? "demo@customer.com";
            Tickets = _db.ServiceRequests.Where(r => r.Email == customerEmail).OrderByDescending(r => r.RequestedAt).ToList();
            if (Tickets.Any(t => t.SatisfactionScore.HasValue))
                AverageSatisfaction = Tickets.Where(t => t.SatisfactionScore.HasValue).Average(t => t.SatisfactionScore.Value);
            else
                AverageSatisfaction = 0;
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
