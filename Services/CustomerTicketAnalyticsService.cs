// Sprint 26.5 Patch Log: CS860x/CS8625/CS1998/CS0219 fixes — Nullability, async, and unused variable corrections for Nova review.
// Sprint 26.6 Patch Log: CS8073 fix — Corrected logic for struct null comparison. DateTime cannot be null, so check for default value instead. Previous comments preserved below.
// Sprint 46.2 – Customer Ticket Analytics Backend
// Sprint 81: Null safety hardening for CustomerTicketAnalyticsService.cs
using Data;
using System;
using System.Linq;

namespace Services
{
    // Sprint 46.2 – Customer Ticket Analytics Backend
    public class CustomerTicketAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public CustomerTicketAnalyticsService(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Sprint 46.2 – Get total ticket count for a customer
        public int GetTicketCountForCustomer(int customerId)
        {
            return _db.ServiceRequests.Count(r => r.Email != null && _db.Customers.Any(c => c.Id == customerId && c.Email == r.Email));
        }

        // Sprint 46.2 – Get average response time (in minutes) for a customer
        public double GetAverageResponseTime(int customerId)
        {
            // No FirstResponseAt in ServiceRequest, so use FirstViewedAt as proxy
            var customer = _db.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null || string.IsNullOrEmpty(customer.Email)) return 0;
            var requests = _db.ServiceRequests.Where(r => r.Email == customer.Email && r.RequestedAt != default && r.FirstViewedAt != null).ToList(); // Fix: Use default(DateTime) for struct null check
            if (!requests.Any()) return 0;
            return requests.Average(r => (r.FirstViewedAt.Value - r.RequestedAt).TotalMinutes);
        }

        // Sprint 46.2 – Get satisfaction rating trend (last 10 tickets, most recent first)
        public int[] GetSatisfactionRatingTrend(int customerId)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null || string.IsNullOrEmpty(customer.Email)) return new int[0];
            return _db.ServiceRequests
                .Where(r => r.Email == customer.Email && r.SatisfactionScore.HasValue)
                .OrderByDescending(r => r.RequestedAt)
                .Take(10)
                .Select(r => r.SatisfactionScore.Value)
                .ToArray();
        }
    }
}
