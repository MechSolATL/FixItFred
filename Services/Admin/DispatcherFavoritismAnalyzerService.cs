using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Data.Models;
using Data;

namespace Services.Admin
{
    public class DispatcherFavoritismAnalyzerService
    {
        private readonly ApplicationDbContext _db;
        private const double AutoFlagThreshold = 0.85;

        public DispatcherFavoritismAnalyzerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<FavoritismAlertLog>> AnalyzeDispatcherPatternsAsync()
        {
            var assignmentLogs = await _db.DispatcherAssignmentLogs.ToListAsync();
            var alerts = new List<FavoritismAlertLog>();
            var grouped = assignmentLogs.GroupBy(x => new { x.DispatcherId, x.TechnicianId });
            foreach (var group in grouped)
            {
                double score = GetFavoritismScore(group.ToList());
                if (score > AutoFlagThreshold)
                {
                    var alert = new FavoritismAlertLog
                    {
                        DispatcherId = group.Key.DispatcherId,
                        TechnicianId = group.Key.TechnicianId,
                        PatternScore = score,
                        FlaggedAt = DateTime.UtcNow,
                        AdminReviewed = false,
                        ResolutionNotes = null
                    };
                    alerts.Add(alert);
                }
            }
            return alerts;
        }

        public double GetFavoritismScore(List<DispatcherAssignmentLog> logs)
        {
            // Example: ratio of assignments to this tech vs all assignments by dispatcher
            if (!logs.Any()) return 0;
            int totalAssignments = _db.DispatcherAssignmentLogs.Count(x => x.DispatcherId == logs[0].DispatcherId);
            int techAssignments = logs.Count;
            return totalAssignments == 0 ? 0 : (double)techAssignments / totalAssignments;
        }

        public async Task FlagOutliers()
        {
            var alerts = await AnalyzeDispatcherPatternsAsync();
            foreach (var alert in alerts)
            {
                // Only add if not already flagged
                bool exists = await _db.FavoritismAlertLogs.AnyAsync(x => x.DispatcherId == alert.DispatcherId && x.TechnicianId == alert.TechnicianId && x.PatternScore == alert.PatternScore);
                if (!exists)
                {
                    _db.FavoritismAlertLogs.Add(alert);
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
