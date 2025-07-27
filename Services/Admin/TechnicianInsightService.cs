using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
{
    public class TechnicianInsightService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianInsightService(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<List<TechnicianKPIViewModel>> GetTechnicianKPIsAsync()
        {
            var technicians = await _db.Technicians.ToListAsync();
            var kpis = new List<TechnicianKPIViewModel>();
            foreach (var tech in technicians)
            {
                var avgResponseTime = await _db.ServiceRequests
                    .Where(r => r.AssignedTechnicianId == tech.Id && r.FirstViewedAt != null && r.RequestedAt != null)
                    .AverageAsync(r => (double?)(r.FirstViewedAt.Value - r.RequestedAt).TotalMinutes) ?? 0;
                var completedCount = await _db.ServiceRequests.CountAsync(r => r.AssignedTechnicianId == tech.Id && r.Status == "Completed");
                var totalCount = await _db.ServiceRequests.CountAsync(r => r.AssignedTechnicianId == tech.Id);
                var completionRate = totalCount > 0 ? (double)completedCount / totalCount : 0;
                var totalEarnings = await _db.TechnicianPayRecords
                    .Where(p => p.TechnicianId == tech.Id)
                    .SumAsync(p => (decimal?)p.TotalPay) ?? 0;
                var activeCerts = await _db.CertificationRecords
                    .CountAsync(c => c.TechnicianId == tech.Id && !c.IsExpired);
                var customerRating = await _db.CustomerReviews
                    .Where(r => r.ServiceRequestId != 0 && _db.ServiceRequests.Any(s => s.Id == r.ServiceRequestId && s.AssignedTechnicianId == tech.Id))
                    .AverageAsync(r => (double?)r.Rating) ?? 0;
                kpis.Add(new TechnicianKPIViewModel
                {
                    TechnicianId = tech.Id,
                    TechnicianName = tech.Name ?? string.Empty,
                    AvgResponseTime = avgResponseTime,
                    CompletionRate = completionRate,
                    TotalEarnings = totalEarnings,
                    ActiveCertifications = activeCerts,
                    CustomerRating = customerRating
                });
            }
            return kpis;
        }

        public async Task LogInsightAsync(int technicianId, string type, string detail)
        {
            var log = new TechnicianInsightLog
            {
                TechnicianId = technicianId,
                InsightType = type ?? string.Empty,
                InsightDetail = detail ?? string.Empty,
                LoggedAt = DateTime.UtcNow
            };
            _db.TechnicianInsightLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
