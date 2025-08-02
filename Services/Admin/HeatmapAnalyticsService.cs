using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Services.Admin
{
    public class HeatmapAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public HeatmapAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogTechnicianCoordinatesAsync(int technicianId, double latitude, double longitude, string serviceType, string zoneTag)
        {
            var log = new TechnicianHeatmapLog
            {
                TechnicianId = technicianId,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTime.UtcNow,
                ServiceType = serviceType,
                ZoneTag = zoneTag
            };
            _db.TechnicianHeatmapLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<IDictionary<string, int>> GetHeatmapDensityAsync(string? zoneTag = null)
        {
            var query = _db.TechnicianHeatmapLogs.AsQueryable();
            if (!string.IsNullOrEmpty(zoneTag))
                query = query.Where(x => x.ZoneTag == zoneTag);
            var density = await query
                .GroupBy(x => x.ZoneTag)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
            return density;
        }

        public async Task<List<string>> DetectCoverageGaps()
        {
            // Example: Find zones with no logs in last 24h
            var since = DateTime.UtcNow.AddHours(-24);
            var allZones = await _db.TechnicianHeatmapLogs.Select(x => x.ZoneTag).Distinct().ToListAsync();
            var activeZones = await _db.TechnicianHeatmapLogs.Where(x => x.Timestamp > since).Select(x => x.ZoneTag).Distinct().ToListAsync();
            return allZones.Except(activeZones).ToList();
        }
    }
}
