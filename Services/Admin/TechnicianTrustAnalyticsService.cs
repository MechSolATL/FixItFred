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

        public async Task<List<MVP_Core.Pages.Admin.TechnicianDropAlertDto>> GetRecentDropAlerts()
        {
            // Find technicians whose trust score dropped by 20+ in last 48h
            var now = DateTime.UtcNow;
            var logs = _db.TechnicianTrustLogs
                .Where(t => t.RecordedAt > now.AddDays(-2))
                .OrderByDescending(t => t.RecordedAt)
                .ToList();
            var drops = new List<MVP_Core.Pages.Admin.TechnicianDropAlertDto>();
            var grouped = logs.GroupBy(l => l.TechnicianId);
            foreach (var g in grouped)
            {
                var recent = g.OrderByDescending(l => l.RecordedAt).Take(2).ToList();
                if (recent.Count == 2 && recent[0].TrustScore < recent[1].TrustScore - 19)
                {
                    var tech = _db.Technicians.FirstOrDefault(t => t.Id == g.Key);
                    if (tech != null)
                    {
                        drops.Add(new MVP_Core.Pages.Admin.TechnicianDropAlertDto
                        {
                            TechnicianId = tech.Id,
                            Name = tech.FullName,
                            PreviousScore = (int)recent[1].TrustScore,
                            CurrentScore = (int)recent[0].TrustScore,
                            City = tech.City ?? string.Empty,
                            ZipCode = tech.ZipCode ?? string.Empty,
                            DropReason = $"Drop of {(int)(recent[1].TrustScore - recent[0].TrustScore)} in 48h"
                        });
                    }
                }
            }
            return await Task.FromResult(drops);
        }
    }
}
