// Sprint 84.8 — Technician Heat Score + Map Overlay
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Data;
using Pages.Admin;

namespace Services.Admin
{
    // Sprint 84.8 — Technician Heat Score + Map Overlay
    public class TechnicianTrustAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianTrustAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TechnicianHeatScoreDto>> GetHeatScoreMapData()
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

        // Sprint 85.0 — Trust Trends Chart Logic + Filters
        public async Task<List<TechnicianTrendPoint>> GetTechnicianTrustTrends(DateTime start, DateTime end, int? technicianId = null)
        {
            var logs = _db.TechnicianTrustLogs
                .Where(l => l.RecordedAt >= start && l.RecordedAt <= end)
                .ToList();
            if (technicianId.HasValue)
                logs = logs.Where(l => l.TechnicianId == technicianId.Value).ToList();
            var techs = _db.Technicians.ToList();
            var points = logs
                .GroupBy(l => new { l.TechnicianId, l.RecordedAt.Date })
                .Select(g => new MVP_Core.Pages.Admin.TechnicianTrendPoint
                {
                    TechnicianId = g.Key.TechnicianId,
                    TechnicianName = techs.FirstOrDefault(t => t.Id == g.Key.TechnicianId)?.FullName ?? $"Tech #{g.Key.TechnicianId}",
                    Date = g.Key.Date,
                    HeatScore = (int)g.Average(l => l.TrustScore)
                })
                .OrderBy(p => p.TechnicianId).ThenBy(p => p.Date)
                .ToList();
            return await Task.FromResult(points);
        }
    }
}
// Sprint 85.0 — Trust Trends Chart Logic + Filters
