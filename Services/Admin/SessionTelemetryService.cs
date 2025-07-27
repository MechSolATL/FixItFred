using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
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