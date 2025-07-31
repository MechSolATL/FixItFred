using System;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
{
    public class ReplayEngineService
    {
        private readonly ApplicationDbContext _db;
        public ReplayEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> CaptureSnapshotAsync(object data, string type, string summary, string createdBy)
        {
            var detailsJson = System.Text.Json.JsonSerializer.Serialize(data);
            var hash = SnapshotHasher.ComputeHash(detailsJson);
            var snapshot = new SystemSnapshotLog
            {
                SnapshotType = type,
                Summary = summary,
                DetailsJson = detailsJson,
                CreatedBy = createdBy,
                SnapshotHash = hash,
                CreatedAt = DateTime.UtcNow
            };
            _db.SystemSnapshotLogs.Add(snapshot);
            await _db.SaveChangesAsync();
            return hash;
        }

        public async Task<bool> ReplaySnapshotAsync(
            string snapshotHash,
            string? triggeredBy,
            DateTime? overrideTimestamp = null,
            string notes = null)
        {
            triggeredBy ??= "system";
            return await Task.FromResult(true);
        }

        public Task QueueRecoveryScenarioAsync(string scenarioId)
            => Task.CompletedTask;
    }
}