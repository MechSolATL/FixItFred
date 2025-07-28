// Sprint 85.6 — Coaching Impact Insights Phase 2
using MVP_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    // Sprint 85.6 — Coaching Impact Insights Phase 2
    public class CoachingLogService
    {
        private readonly List<CoachingLogEntry> _entries = new(); // Replace with DB in production

        // Category weights for trust improvement
        private static readonly Dictionary<string, int> CategoryWeights = new()
        {
            { "Skill", 3 },
            { "Timeliness", 2 },
            { "Performance", 2 },
            { "Attitude", 1 },
            { "Communication", 2 }
        };

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

        // Sprint 85.6 — Coaching Impact Insights Phase 2
        // Returns the trust improvement score for a single entry
        public int GetTrustImprovement(CoachingLogEntry entry)
        {
            if (CategoryWeights.TryGetValue(entry.Category, out int weight))
                return weight;
            return 1; // Default weight
        }

        // Returns the average trust improvement per technician
        public double GetAverageTrustImprovementPerTechnician()
        {
            if (!_entries.Any()) return 0;
            var grouped = _entries.GroupBy(e => e.TechnicianId)
                .Select(g => g.Sum(e => GetTrustImprovement(e)));
            return grouped.Any() ? grouped.Average() : 0;
        }

        // Returns trust improvement for a technician
        public int GetTotalTrustImprovementForTechnician(int technicianId)
        {
            return _entries.Where(e => e.TechnicianId == technicianId).Sum(GetTrustImprovement);
        }
    }
}
