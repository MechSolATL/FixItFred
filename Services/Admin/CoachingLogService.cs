// Sprint 85.2 — Coaching Logbook System
using MVP_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    // Sprint 85.2 — Coaching Logbook System
    public class CoachingLogService
    {
        private readonly List<CoachingLogEntry> _entries = new(); // Replace with DB in production

        public void AddEntry(CoachingLogEntry entry)
        {
            entry.Id = _entries.Count > 0 ? _entries.Max(e => e.Id) + 1 : 1;
            entry.Timestamp = DateTime.UtcNow;
            _entries.Add(entry);
        }

        public List<CoachingLogEntry> GetLogsForTechnician(int technicianId)
        {
            return _entries.Where(e => e.TechnicianId == technicianId).OrderByDescending(e => e.Timestamp).ToList();
        }

        public List<CoachingLogEntry> FilterByDate(DateTime start, DateTime end)
        {
            return _entries.Where(e => e.Timestamp >= start && e.Timestamp <= end).OrderByDescending(e => e.Timestamp).ToList();
        }

        public List<CoachingLogEntry> FilterByCategory(string category)
        {
            return _entries.Where(e => e.Category == category).OrderByDescending(e => e.Timestamp).ToList();
        }
    }
}
