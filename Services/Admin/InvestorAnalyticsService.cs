using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Services.Admin
{
    public class InvestorAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public InvestorAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        // 1. Total Revenue (by month, service type)
        public async Task<List<RevenueSummary>> GetRevenueByMonthAsync(DateTime? start, DateTime? end, string? serviceType)
        {
            var query = _db.BillingInvoiceRecords.AsQueryable();
            if (start.HasValue) query = query.Where(x => x.InvoiceDate >= start);
            if (end.HasValue) query = query.Where(x => x.InvoiceDate <= end);
            if (!string.IsNullOrEmpty(serviceType))
                query = query.Where(x => x.Status == "Paid" && x.CustomerName.Contains(serviceType)); // ServiceType mapping may need adjustment
            var result = await query
                .GroupBy(x => new { x.InvoiceDate.Month, x.InvoiceDate.Year })
                .Select(g => new RevenueSummary
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.AmountTotal - (x.AmountPaid ?? 0))
                }).OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            return result;
        }

        public class RevenueSummary
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        // 2. SLA Completion %
        public async Task<double> GetSlaCompletionRateAsync(DateTime? start, DateTime? end, string? zone, string? serviceType)
        {
            var query = _db.ServiceRequests.AsQueryable();
            if (start.HasValue) query = query.Where(x => x.RequestedAt >= start);
            if (end.HasValue) query = query.Where(x => x.RequestedAt <= end);
            if (!string.IsNullOrEmpty(zone)) query = query.Where(x => x.Zip == zone);
            if (!string.IsNullOrEmpty(serviceType)) query = query.Where(x => x.ServiceType == serviceType);
            var total = await query.CountAsync();
            var completed = await query.Where(x => x.Status == "Completed").CountAsync();
            return total == 0 ? 0 : (double)completed / total * 100.0;
        }

        // 3. Average Review by service type
        public async Task<List<ReviewSummary>> GetAverageReviewByServiceTypeAsync(DateTime? start, DateTime? end)
        {
            var query = _db.CustomerReviews.AsQueryable();
            if (start.HasValue) query = query.Where(x => x.SubmittedAt >= start);
            if (end.HasValue) query = query.Where(x => x.SubmittedAt <= end);
            var result = await query
                .GroupBy(x => x.ServiceRequestId)
                .Select(g => new
                {
                    ServiceRequestId = g.Key,
                    AvgRating = g.Average(x => x.Rating)
                }).ToListAsync();
            var serviceTypes = _db.ServiceRequests.ToDictionary(x => x.Id, x => x.ServiceType);
            return result.Select(x => new ReviewSummary
            {
                ServiceType = serviceTypes.ContainsKey(x.ServiceRequestId) ? serviceTypes[x.ServiceRequestId] : "Unknown",
                AverageRating = x.AvgRating
            }).GroupBy(x => x.ServiceType).Select(g => new ReviewSummary
            {
                ServiceType = g.Key,
                AverageRating = g.Average(x => x.AverageRating)
            }).ToList();
        }

        public class ReviewSummary
        {
            public string ServiceType { get; set; } = string.Empty;
            public double AverageRating { get; set; }
        }

        // 4. Technician Payout + Bonus Summary
        public async Task<List<TechnicianPayoutSummary>> GetTechnicianPayoutSummaryAsync(DateTime? start, DateTime? end)
        {
            var payQuery = _db.TechnicianPayRecords.AsQueryable();
            if (start.HasValue) payQuery = payQuery.Where(x => x.PeriodStart >= start);
            if (end.HasValue) payQuery = payQuery.Where(x => x.PeriodEnd <= end);
            var bonusQuery = _db.TechnicianBonusLogs.AsQueryable();
            if (start.HasValue) bonusQuery = bonusQuery.Where(x => x.AwardedAt >= start);
            if (end.HasValue) bonusQuery = bonusQuery.Where(x => x.AwardedAt <= end);
            var paySummary = await payQuery
                .GroupBy(x => x.TechnicianId)
                .Select(g => new
                {
                    TechnicianId = g.Key,
                    TotalPay = g.Sum(x => x.TotalPay),
                    Commission = g.Sum(x => x.CommissionEarned)
                }).ToListAsync();
            var bonusSummary = await bonusQuery
                .GroupBy(x => x.TechnicianId)
                .Select(g => new
                {
                    TechnicianId = g.Key,
                    TotalBonus = g.Sum(x => x.Amount)
                }).ToListAsync();
            var result = paySummary.Select(x => new TechnicianPayoutSummary
            {
                TechnicianId = x.TechnicianId,
                TotalPay = x.TotalPay,
                Commission = x.Commission,
                TotalBonus = bonusSummary.FirstOrDefault(b => b.TechnicianId == x.TechnicianId)?.TotalBonus ?? 0
            }).ToList();
            return result;
        }

        public class TechnicianPayoutSummary
        {
            public int TechnicianId { get; set; }
            public decimal TotalPay { get; set; }
            public decimal Commission { get; set; }
            public decimal TotalBonus { get; set; }
        }

        // 5. Job Volume & Conversion Funnel
        public async Task<JobFunnelSummary> GetJobFunnelSummaryAsync(DateTime? start, DateTime? end, string? serviceType)
        {
            var query = _db.ServiceRequests.AsQueryable();
            if (start.HasValue) query = query.Where(x => x.RequestedAt >= start);
            if (end.HasValue) query = query.Where(x => x.RequestedAt <= end);
            if (!string.IsNullOrEmpty(serviceType)) query = query.Where(x => x.ServiceType == serviceType);
            var total = await query.CountAsync();
            var scheduled = await query.Where(x => x.Status == "Scheduled").CountAsync();
            var completed = await query.Where(x => x.Status == "Completed").CountAsync();
            var cancelled = await query.Where(x => x.Status == "Cancelled").CountAsync();
            return new JobFunnelSummary
            {
                Total = total,
                Scheduled = scheduled,
                Completed = completed,
                Cancelled = cancelled
            };
        }

        public class JobFunnelSummary
        {
            public int Total { get; set; }
            public int Scheduled { get; set; }
            public int Completed { get; set; }
            public int Cancelled { get; set; }
        }
    }
}
