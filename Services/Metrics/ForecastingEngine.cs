using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Models.ViewModels;
using MVP_Core.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace MVP_Core.Services.Metrics
{
    public class ForecastingEngine
    {
        private readonly ApplicationDbContext _db;
        public ForecastingEngine(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ForecastMetricsViewModel> GetForecastAsync()
        {
            var nextWeek = DateTime.UtcNow.AddDays(7);
            var projectedRequests = await _db.ServiceRequests.CountAsync(r => r.CreatedAt >= DateTime.UtcNow && r.CreatedAt < nextWeek);
            // Use ClosedAt for completed jobs
            var avgDelay = await _db.ServiceRequests.Where(r => r.ClosedAt != null).AverageAsync(r => EF.Functions.DateDiffMinute(r.CreatedAt, r.ClosedAt.Value));
            var projectedRevenue = await _db.BillingInvoiceRecords.Where(i => i.InvoiceDate >= DateTime.UtcNow && i.InvoiceDate < nextWeek).SumAsync(i => (decimal?)i.AmountTotal) ?? 0;
            var techGap = await _db.Technicians.CountAsync(t => t.IsActive && t.CurrentJobCount >= t.MaxJobCapacity);

            return new ForecastMetricsViewModel
            {
                ProjectedRequests = projectedRequests,
                AverageDelayMinutes = (int)Math.Round(avgDelay),
                ProjectedRevenue = projectedRevenue,
                TechnicianGap = techGap
            };
        }

        // Sprint 89.1: Detect overdue invoices
        public async Task<int> DetectOverdueInvoicesAsync()
        {
            var overdue = await _db.BillingInvoiceRecords.CountAsync(i => !i.IsPaid && i.IsOverdue);
            return overdue;
        }

        // Sprint 89.1: Late payment trend (last 30 days)
        public async Task<double> LatePaymentTrendAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var latePaid = await _db.BillingInvoiceRecords.CountAsync(i => i.PaidDate != null && i.PaidDate > i.DueDate && i.PaidDate >= thirtyDaysAgo);
            var totalPaid = await _db.BillingInvoiceRecords.CountAsync(i => i.PaidDate != null && i.PaidDate >= thirtyDaysAgo);
            return totalPaid > 0 ? (100.0 * latePaid / totalPaid) : 0.0;
        }
    }
}
