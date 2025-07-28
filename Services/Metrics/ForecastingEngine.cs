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
            // Example logic, replace with real predictive analytics
            var nextWeek = DateTime.UtcNow.AddDays(7);
            var projectedRequests = await _db.ServiceRequests.CountAsync(r => r.CreatedAt >= DateTime.UtcNow && r.CreatedAt < nextWeek);
            var avgDelay = await _db.ServiceRequests.Where(r => r.CompletedAt != null).AverageAsync(r => EF.Functions.DateDiffMinute(r.CreatedAt, r.CompletedAt.Value));
            var projectedRevenue = await _db.BillingInvoiceRecords.Where(i => i.InvoiceDate >= DateTime.UtcNow && i.InvoiceDate < nextWeek).SumAsync(i => (decimal?)i.Amount) ?? 0;
            var techGap = await _db.Technicians.CountAsync(t => t.IsActive && t.IsAvailable == false);

            return new ForecastMetricsViewModel
            {
                ProjectedRequests = projectedRequests,
                AverageDelayMinutes = (int)Math.Round(avgDelay),
                ProjectedRevenue = projectedRevenue,
                TechnicianGap = techGap
            };
        }
    }
}
