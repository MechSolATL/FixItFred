using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
{
    public class GpsDriftDetectorService
    {
        private readonly ApplicationDbContext _db;
        public GpsDriftDetectorService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogDriftEventAsync(GpsDriftEventLog log)
        {
            _db.GpsDriftEventLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public bool DetectDrift(double scheduledLat, double scheduledLng, double actualLat, double actualLng, double thresholdMeters)
        {
            // Haversine formula for distance
            double R = 6371000; // meters
            double dLat = (actualLat - scheduledLat) * Math.PI / 180.0;
            double dLon = (actualLng - scheduledLng) * Math.PI / 180.0;
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(scheduledLat * Math.PI / 180.0) * Math.Cos(actualLat * Math.PI / 180.0) * Math.Sin(dLon/2) * Math.Sin(dLon/2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            double distance = R * c;
            return distance > thresholdMeters;
        }

        public async Task<List<GpsDriftEventLog>> GetRecentDriftEvents(DateTime? from = null, int? technicianId = null, double? minDistance = null)
        {
            var query = _db.GpsDriftEventLogs.AsQueryable();
            if (from.HasValue)
                query = query.Where(e => e.DriftDetectedAt >= from.Value);
            if (technicianId.HasValue)
                query = query.Where(e => e.TechnicianId == technicianId.Value);
            if (minDistance.HasValue)
                query = query.Where(e => e.DistanceDriftMeters >= minDistance.Value);
            return await query.OrderByDescending(e => e.DriftDetectedAt).ToListAsync();
        }
    }
}