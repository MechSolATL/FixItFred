using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Admin
{
    public class TechnicianAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public double GetOfflineHoursLast30Days(int technicianId)
        {
            var since = DateTime.UtcNow.AddDays(-30);
            var sessions = _db.TechnicianOfflineSessions
                .Where(s => s.TechnicianId == technicianId && s.StartTime >= since)
                .ToList();
            double totalHours = 0;
            foreach (var s in sessions)
            {
                var end = s.EndTime ?? DateTime.UtcNow;
                totalHours += (end - s.StartTime).TotalHours;
            }
            return Math.Round(totalHours, 2);
        }

        public double GetOfflinePercentVsJobs(int technicianId)
        {
            var since = DateTime.UtcNow.AddDays(-30);
            var sessions = _db.TechnicianOfflineSessions
                .Where(s => s.TechnicianId == technicianId && s.StartTime >= since)
                .ToList();
            var jobs = _db.ServiceRequests
                .Where(r => r.AssignedTechnicianId == technicianId && r.CreatedAt >= since)
                .ToList();
            double offlineHours = 0;
            foreach (var s in sessions)
            {
                var end = s.EndTime ?? DateTime.UtcNow;
                offlineHours += (end - s.StartTime).TotalHours;
            }
            double jobCount = jobs.Count;
            return jobCount > 0 ? Math.Round(offlineHours / (jobCount * 8) * 100, 2) : 0; // Assume 8h/job
        }

        public int GetOfflineIncidentCount(int technicianId)
        {
            var since = DateTime.UtcNow.AddDays(-30);
            return _db.TechnicianOfflineSessions.Count(s => s.TechnicianId == technicianId && s.StartTime >= since);
        }

        public List<string> GetFrequentOfflineZones(int technicianId)
        {
            var since = DateTime.UtcNow.AddDays(-30);
            return _db.TechnicianOfflineSessions
                .Where(s => s.TechnicianId == technicianId && s.StartTime >= since && !string.IsNullOrEmpty(s.LocationZip))
                .GroupBy(s => s.LocationZip)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key ?? "Unknown")
                .ToList();
        }

        public double GetSlaImpactFromOffline(int technicianId)
        {
            var since = DateTime.UtcNow.AddDays(-30);
            var sessions = _db.TechnicianOfflineSessions
                .Where(s => s.TechnicianId == technicianId && s.StartTime >= since)
                .ToList();
            var jobs = _db.ServiceRequests
                .Where(r => r.AssignedTechnicianId == technicianId && r.CreatedAt >= since)
                .ToList();
            int slaBreaches = jobs.Count(j => j.IsEscalated);
            double offlineHours = sessions.Sum(s => ((s.EndTime ?? DateTime.UtcNow) - s.StartTime).TotalHours);
            return jobs.Count > 0 ? Math.Round(slaBreaches / (double)jobs.Count * 100, 2) : 0;
        }
    }
}
