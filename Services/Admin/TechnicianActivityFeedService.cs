using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class TechnicianActivityFeedService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianActivityFeedService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogActivityAsync(int technicianId, string activityType, double latitude, double longitude, string metaDataJson, bool isVisible, string sessionId, string routeGroupTag)
        {
            var log = new TechnicianActivityFeedLog
            {
                TechnicianId = technicianId,
                ActivityType = activityType,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTime.UtcNow,
                MetaDataJson = metaDataJson,
                IsVisible = isVisible,
                SessionId = sessionId,
                RouteGroupTag = routeGroupTag
            };
            _db.TechnicianActivityFeedLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<List<TechnicianActivityFeedLog>> GetLiveFeedAsync(int technicianId)
        {
            var now = DateTime.UtcNow;
            return await _db.TechnicianActivityFeedLogs
                .Where(x => x.TechnicianId == technicianId && x.IsVisible && (now - x.Timestamp).TotalMinutes < 60)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<List<TechnicianActivityFeedLog>> GetGeoTrailSessionAsync(string sessionId)
        {
            return await _db.TechnicianActivityFeedLogs
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<List<TechnicianActivityFeedLog>> ReplayGeoTrailAsync(string sessionId, int stepInterval)
        {
            var logs = await _db.TechnicianActivityFeedLogs
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
            // Return logs at stepInterval (e.g., every Nth log)
            return logs.Where((x, i) => i % stepInterval == 0).ToList();
        }
    }
}
