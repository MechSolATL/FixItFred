using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Data;
using Pages.Admin;

namespace Services.Admin
{
    public class ExecutiveSnapshotService
    {
        private readonly ApplicationDbContext _db;
        public ExecutiveSnapshotService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ExecutiveSnapshotViewModel> GetSnapshotAsync()
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var weekAgo = today.AddDays(-7);

            // ServiceRequest Flow
            var requestsToday = await _db.ServiceRequests.Where(r => r.RequestedAt >= today).ToListAsync();
            var totalRequests = requestsToday.Count;
            var completedRequests = requestsToday.Count(r => r.Status == "Completed");
            var invoicedRequests = requestsToday.Count(r => r.InvoiceCompletedAt != null);

            // Avg. Time to Completion (in hours)
            var completedToday = requestsToday.Where(r => r.Status == "Completed" && r.ClosedAt != null && r.RequestedAt != null);
            double avgCompletionTime = completedToday.Any()
                ? completedToday.Average(r => (r.ClosedAt.Value - r.RequestedAt).TotalHours)
                : 0;

            // Missed/Rescheduled Ratio
            int rescheduled = requestsToday.Count(r => r.Status == "Rescheduled");
            double missedRescheduleRatio = totalRequests > 0 ? (double)rescheduled / totalRequests : 0;

            // Top Performing Technicians (by job count, then revenue)
            var topTechs = requestsToday
                .Where(r => r.AssignedTechnicianId != null && r.Status == "Completed")
                .GroupBy(r => r.AssignedTechnicianId)
                .Select(g => new
                {
                    TechnicianId = g.Key.Value,
                    JobsCompleted = g.Count(),
                    Revenue = g.Sum(r => r.TechnicianCommission ?? 0)
                })
                .OrderByDescending(t => t.JobsCompleted)
                .ThenByDescending(t => t.Revenue)
                .Take(3)
                .ToList();
            var techNames = await _db.Technicians.Where(t => topTechs.Select(x => x.TechnicianId).Contains(t.Id)).ToListAsync();
            var topTechPerformances = topTechs.Select(t => new TopTechPerformance
            {
                TechnicianName = techNames.FirstOrDefault(n => n.Id == t.TechnicianId)?.FullName ?? $"Tech #{t.TechnicianId}",
                JobsCompleted = t.JobsCompleted,
                RevenueGenerated = t.Revenue
            }).ToList();

            // Historical trend comparison
            var completedYesterday = await _db.ServiceRequests.CountAsync(r => r.RequestedAt >= yesterday && r.RequestedAt < today && r.Status == "Completed");
            double percentChange = completedYesterday > 0 ? ((double)completedRequests - completedYesterday) / completedYesterday * 100 : 0;
            var trend = new HistoricalComparison
            {
                YesterdayCompleted = completedYesterday,
                PercentChange = percentChange
            };

            // Alerts
            var overdueInvoices = await _db.BillingInvoiceRecords.CountAsync(i => !i.IsPaid && i.IsOverdue);
            var toolsNotReturned = await _db.ToolInventories.CountAsync(t => t.IsActive && t.AssignedTechId == null);
            var unresolvedFlags = await _db.FlaggedCustomers.CountAsync();
            var criticalAlerts = new List<string>();
            if (overdueInvoices > 0) criticalAlerts.Add($"Overdue invoices: {overdueInvoices}");
            if (toolsNotReturned > 0) criticalAlerts.Add($"Tools not returned: {toolsNotReturned}");
            if (unresolvedFlags > 0) criticalAlerts.Add($"Unresolved customer flags: {unresolvedFlags}");

            return new ExecutiveSnapshotViewModel
            {
                SnapshotDate = today.ToString("yyyy-MM-dd"),
                TotalRequests = totalRequests,
                CompletedRequests = completedRequests,
                InvoicedRequests = invoicedRequests,
                AverageCompletionTimeHours = Math.Round(avgCompletionTime, 2),
                MissedRescheduleRatio = Math.Round(missedRescheduleRatio, 2),
                TopTechnicians = topTechPerformances,
                CriticalAlerts = criticalAlerts,
                Trend = trend
            };
        }
    }
}
