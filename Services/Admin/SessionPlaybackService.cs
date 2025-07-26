using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    // Sprint 70.5: Session Playback Service
    public class SessionPlaybackService
    {
        private readonly ApplicationDbContext _db;
        public SessionPlaybackService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Record a session event
        public async Task RecordEventAsync(string sessionId, string eventData, string user)
        {
            var log = new SessionPlaybackLog
            {
                SessionId = sessionId,
                EventDataJson = eventData,
                RecordedBy = user,
                RecordedAt = DateTime.UtcNow
            };
            _db.SessionPlaybackLogs.Add(log);
            await _db.SaveChangesAsync();
        }
        // Get all events for a session
        public async Task<List<SessionPlaybackLog>> GetSessionEventsAsync(string sessionId)
        {
            return await _db.SessionPlaybackLogs
                .AsNoTracking()
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.RecordedAt)
                .ToListAsync();
        }
    }
}
