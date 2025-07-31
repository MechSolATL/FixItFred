using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Data;

namespace Services.Admin
{
    public class UptimeAnalyzerService
    {
        private readonly ApplicationDbContext _db;
        public UptimeAnalyzerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TechnicianSessionTelemetry>> AnalyzeUptimeAsync(int? technicianId = null, string? region = null)
        {
            var query = _db.TechnicianSessionTelemetries.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(t => t.TechnicianId == technicianId.Value);
            if (!string.IsNullOrEmpty(region))
                query = query.Where(t => t.Notes.Contains(region));
            return await query.OrderByDescending(t => t.LoggedAt).ToListAsync();
        }

        public async Task<List<UptimeHeartbeatLog>> DetectDropoutsAsync(int? technicianId = null)
        {
            var query = _db.UptimeHeartbeatLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(h => h.TechnicianId == technicianId.Value);
            return await query.Where(h => !h.IsActive || h.BatteryLevel < 10).OrderByDescending(h => h.HeartbeatAt).ToListAsync();
        }

        public async Task<List<UptimeHeartbeatLog>> GetHeatSignatureMap(string? region = null)
        {
            var query = _db.UptimeHeartbeatLogs.AsQueryable();
            if (!string.IsNullOrEmpty(region))
                query = query.Where(h => h.Region == region);
            return await query.OrderByDescending(h => h.HeartbeatAt).ToListAsync();
        }
    }
}