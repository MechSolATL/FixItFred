// Sprint 84.8 — Technician Heat Score + Map Overlay
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Fix: Use System for DateTime

namespace MVP_Core.Services.Admin
{
    // Sprint 84.8 — Technician Heat Score + Map Overlay
    public class TechnicianTrustAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianTrustAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<MVP_Core.Pages.Admin.TechnicianHeatScoreDto>> GetHeatScoreMapData()
        {
            // Join Technicians with latest TrustLog, fallback to TrustScore
            var techs = _db.Technicians.ToList();
            var trustLogs = _db.TechnicianTrustLogs
                .GroupBy(t => t.TechnicianId)
                .Select(g => g.OrderByDescending(t => t.RecordedAt).FirstOrDefault())
                .ToList();
            var result = techs.Select(t =>
            {
                var log = trustLogs.FirstOrDefault(l => l.TechnicianId == t.Id);
                int heat = log != null ? (int)log.TrustScore : t.TrustScore;
                return new MVP_Core.Pages.Admin.TechnicianHeatScoreDto
                {
                    TechnicianId = t.Id,
                    Name = t.FullName,
                    HeatScore = heat,
                    City = t.City ?? string.Empty,
                    ZipCode = t.ZipCode ?? string.Empty // Use ZipCode from Technician
                };
            }).ToList();
            return await Task.FromResult(result);
        }
    }
}
