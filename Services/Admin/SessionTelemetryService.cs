using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Data;

namespace Services.Admin
{
    public class SessionTelemetryService
    {
        private readonly ApplicationDbContext _db;
        public SessionTelemetryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task StartSessionTrackingAsync(TechnicianSessionTelemetry telemetry)
        {
            _db.TechnicianSessionTelemetries.Add(telemetry);
            await _db.SaveChangesAsync();
        }

        public async Task LogHeartbeatAsync(UptimeHeartbeatLog heartbeat)
        {
            _db.UptimeHeartbeatLogs.Add(heartbeat);
            await _db.SaveChangesAsync();
        }

        public async Task EndSessionTrackingAsync(int telemetryId, DateTime endTime)
        {
            var telemetry = await _db.TechnicianSessionTelemetries.FindAsync(telemetryId);
            if (telemetry != null)
            {
                telemetry.EndTime = endTime;
                telemetry.TotalDurationMinutes = (int)(endTime - telemetry.StartTime).TotalMinutes;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<string> GetSessionHealth(string sessionId)
        {
            var tele = await _db.TechnicianSessionTelemetries.FirstOrDefaultAsync(t => t.SessionId == sessionId);
            if (tele == null) return "Unknown";
            if (tele.Interruptions > 3 || tele.SignalStrength < 2) return "Red";
            if (tele.Interruptions > 0 || tele.SignalStrength < 4) return "Yellow";
            return "Green";
        }
    }
}