using System;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Admin
{
    public class ReplayEngineService
    {
        private readonly ApplicationDbContext _db;
        public ReplayEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ReplaySnapshotAsync(string snapshotHash, string triggeredBy, DateTime? overrideTimestamp = null, string notes = null)
        {
            var snapshot = await _db.SystemSnapshotLogs.FirstOrDefaultAsync(s => s.SnapshotHash == snapshotHash);
            if (snapshot == null) return false;
            // Simulate replay logic
            var replayLog = new ReplayAuditLog
            {
                Timestamp = overrideTimestamp ?? DateTime.UtcNow,
                SnapshotHash = snapshotHash,
                TriggeredBy = triggeredBy,
                Success = true, // Assume success for now
                Notes = notes
            };
            _db.ReplayAuditLogs.Add(replayLog);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> QueueRecoveryScenarioAsync(string scenarioName, string triggeredBy, DateTime scheduledForUtc, string snapshotHash, string? notes = null)
        {
            var scenario = new RecoveryScenarioLog
            {
                ScenarioName = scenarioName,
                TriggeredBy = triggeredBy,
                ScheduledForUtc = scheduledForUtc,
                Executed = false,
                SnapshotHash = snapshotHash,
                Notes = notes
            };
            _db.RecoveryScenarioLogs.Add(scenario);
            await _db.SaveChangesAsync();
            return scenario.Id;
        }

        public async Task<int> RunScheduledScenariosAsync()
        {
            var now = DateTime.UtcNow;
            var scenarios = _db.RecoveryScenarioLogs.Where(s => !s.Executed && s.ScheduledForUtc <= now).ToList();
            int executedCount = 0;
            foreach (var scenario in scenarios)
            {
                var success = await ReplaySnapshotAsync(scenario.SnapshotHash, scenario.TriggeredBy, now, scenario.Notes);
                scenario.Executed = true;
                scenario.ExecutedAtUtc = now;
                scenario.OutcomeSummary = success ? "Replay succeeded" : "Replay failed";
                _db.RecoveryScenarioLogs.Update(scenario);
                executedCount++;
            }
            await _db.SaveChangesAsync();
            return executedCount;
        }

        public IQueryable<ReplayAuditLog> GetReplayLogs() => _db.ReplayAuditLogs.AsQueryable();
        public IQueryable<RecoveryScenarioLog> GetScheduledScenarios() => _db.RecoveryScenarioLogs.AsQueryable();
    }
}