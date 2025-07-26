using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Linq;

namespace MVP_Core.Services
{
    public class OfflineZoneTracker
    {
        private readonly ApplicationDbContext _db;
        public OfflineZoneTracker(ApplicationDbContext db)
        {
            _db = db;
        }
        public void LogSyncFailure(int technicianId, string zipCode, double lat, double lng, string? notes = null)
        {
            var zone = _db.OfflineZoneHeatmaps.FirstOrDefault(z => z.ZipCode == zipCode && Math.Abs(z.Latitude - lat) < 0.001 && Math.Abs(z.Longitude - lng) < 0.001);
            if (zone == null)
            {
                zone = new OfflineZoneHeatmap
                {
                    TechnicianId = technicianId,
                    ZipCode = zipCode,
                    Latitude = lat,
                    Longitude = lng,
                    FailureCount = 1,
                    LastFailureAt = DateTime.UtcNow,
                    Notes = notes
                };
                _db.OfflineZoneHeatmaps.Add(zone);
            }
            else
            {
                zone.FailureCount++;
                zone.LastFailureAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(notes))
                    zone.Notes = notes;
            }
            _db.SaveChanges();
        }
        public bool IsHighFailureZone(string zipCode, double lat, double lng, int threshold = 3)
        {
            var zone = _db.OfflineZoneHeatmaps.FirstOrDefault(z => z.ZipCode == zipCode && Math.Abs(z.Latitude - lat) < 0.001 && Math.Abs(z.Longitude - lng) < 0.001);
            return zone != null && zone.FailureCount >= threshold;
        }
        public IQueryable<OfflineZoneHeatmap> GetAllZones() => _db.OfflineZoneHeatmaps;
    }
}
