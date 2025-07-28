using System;
using System.Collections.Generic;
using System.Linq;
using MVP_Core.Models;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Services.Admin
{
    public class BehaviorPatternEngine
    {
        private readonly ApplicationDbContext _db;
        public BehaviorPatternEngine(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ViolationInsightModel> AnalyzeTechnicianBehavior(int? technicianId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _db.TechnicianBehaviorLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value);
            if (from.HasValue)
                query = query.Where(x => x.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.Timestamp <= to.Value);

            var logs = query.ToList();
            var insights = new List<ViolationInsightModel>();

            // Example: Frequent Backdating
            var backdating = logs.Where(x => x.BehaviorType.Contains("Backdate", StringComparison.OrdinalIgnoreCase)).ToList();
            if (backdating.Count >= 3)
            {
                insights.Add(new ViolationInsightModel
                {
                    TechnicianId = technicianId ?? 0,
                    PatternType = ViolationPatternType.FrequentBackdating,
                    ConfidenceScore = Math.Min(1.0, backdating.Count / 5.0),
                    SummaryText = $"{backdating.Count} backdating events detected.",
                    StartDate = backdating.Min(x => x.Timestamp),
                    EndDate = backdating.Max(x => x.Timestamp)
                });
            }

            // Example: GPS Mismatch
            var gpsMismatch = logs.Where(x => x.BehaviorType.Contains("GPS", StringComparison.OrdinalIgnoreCase) || x.BehaviorType.Contains("Location", StringComparison.OrdinalIgnoreCase)).ToList();
            if (gpsMismatch.Count >= 2)
            {
                insights.Add(new ViolationInsightModel
                {
                    TechnicianId = technicianId ?? 0,
                    PatternType = ViolationPatternType.GpsMismatch,
                    ConfidenceScore = Math.Min(1.0, gpsMismatch.Count / 4.0),
                    SummaryText = $"{gpsMismatch.Count} GPS/location mismatches detected.",
                    StartDate = gpsMismatch.Min(x => x.Timestamp),
                    EndDate = gpsMismatch.Max(x => x.Timestamp)
                });
            }

            // Example: Consecutive Missing Uploads
            var missingUploads = logs.Where(x => x.BehaviorType.Contains("Upload", StringComparison.OrdinalIgnoreCase) && x.Description.Contains("missing", StringComparison.OrdinalIgnoreCase)).ToList();
            if (missingUploads.Count >= 2)
            {
                insights.Add(new ViolationInsightModel
                {
                    TechnicianId = technicianId ?? 0,
                    PatternType = ViolationPatternType.ConsecutiveMissingUploads,
                    ConfidenceScore = Math.Min(1.0, missingUploads.Count / 3.0),
                    SummaryText = $"{missingUploads.Count} missing uploads on consecutive jobs.",
                    StartDate = missingUploads.Min(x => x.Timestamp),
                    EndDate = missingUploads.Max(x => x.Timestamp)
                });
            }

            // Add more pattern detection as needed
            return insights;
        }
    }
}
