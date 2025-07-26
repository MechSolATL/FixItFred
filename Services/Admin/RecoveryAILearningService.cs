using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Helpers;

namespace Services.Admin
{
    public class RecoveryAILearningService
    {
        private readonly ApplicationDbContext _db;
        public RecoveryAILearningService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Analyzes past recovery scenarios and logs frequent error patterns
        public async Task<List<RecoveryLearningLog>> AnalyzeRecoveryPatternsAsync()
        {
            var logs = await _db.RecoveryScenarioLogs.Include(x => x.ServiceRequest).ToListAsync();
            var grouped = logs
                .GroupBy(l => new { l.ScenarioName, l.SnapshotHash })
                .Select(g => new
                {
                    TriggerSignature = g.Key.ScenarioName,
                    PatternHash = g.Key.SnapshotHash,
                    Outcome = g.GroupBy(x => x.OutcomeSummary).OrderByDescending(x => x.Count()).FirstOrDefault()?.Key ?? "Unknown",
                    Count = g.Count(),
                    LastRecorded = g.Max(x => x.ExecutedAtUtc ?? x.ScheduledForUtc),
                    ErrorObject = g.FirstOrDefault(),
                    ServiceRequest = g.FirstOrDefault()?.ServiceRequest
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var learningLogs = new List<RecoveryLearningLog>();
            foreach (var item in grouped)
            {
                var log = new RecoveryLearningLog
                {
                    TriggerSignature = item.TriggerSignature,
                    PatternHash = item.PatternHash,
                    Outcome = item.Outcome,
                    RecordedAt = item.LastRecorded,
                    SourceModule = "SystemDiagnostics",
                    TriggerContextJson = TraceCaptureHelper.CaptureContext(item.ErrorObject),
                    LinkedRequestId = item.ServiceRequest?.Id
                };
                learningLogs.Add(log);
            }
            // Optionally persist new learning logs
            // _db.RecoveryLearningLogs.AddRange(learningLogs);
            // await _db.SaveChangesAsync();
            return learningLogs;
        }

        // Suggests triggers for auto-repair based on learned patterns
        public async Task<List<string>> SuggestAutoRepairTriggers()
        {
            var topTriggers = await _db.RecoveryLearningLogs
                .OrderByDescending(l => l.RecordedAt)
                .Select(l => l.TriggerSignature)
                .Distinct()
                .Take(5)
                .ToListAsync();
            return topTriggers;
        }
    }
}
