using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class DelayMatrixService
    {
        private readonly ApplicationDbContext _db;
        public DelayMatrixService(ApplicationDbContext db) { _db = db; }

        public async Task<List<DepartmentDelayLog>> GetDelaysAsync(int minMinutes = 0)
        {
            return await _db.Set<DepartmentDelayLog>()
                .Where(d => EF.Functions.DateDiffMinute(d.DelayStart, d.DelayEnd ?? DateTime.UtcNow) > minMinutes)
                .OrderByDescending(d => d.DelayStart)
                .ToListAsync();
        }
        public async Task LogDelayAsync(DepartmentDelayLog log)
        {
            _db.Set<DepartmentDelayLog>().Add(log);
            await _db.SaveChangesAsync();
        }
        public async Task<int> ComputeSeverityAsync(int delayMinutes, int escalationLevel)
        {
            // Example: severity = delayMinutes * escalationLevel
            return await Task.FromResult(delayMinutes * (escalationLevel > 0 ? escalationLevel : 1));
        }
    }

    public class TrustCascadeEngine
    {
        private readonly ApplicationDbContext _db;
        public TrustCascadeEngine(ApplicationDbContext db) { _db = db; }

        public async Task<List<TrustCascadeLog>> GetTrustCascadesAsync()
        {
            // Ensure only CreatedAt is used, not CreatedAtAF
            return await _db.Set<TrustCascadeLog>().OrderByDescending(t => t.CreatedAt).ToListAsync();
        }
        public async Task LogCascadeAsync(TrustCascadeLog log)
        {
            _db.Set<TrustCascadeLog>().Add(log);
            await _db.SaveChangesAsync();
        }
        public double CalculateTrustDecay(int hopCount)
        {
            // Example: trust decay = 1 - (0.85 ^ hopCount)
            return 1 - Math.Pow(0.85, hopCount);
        }
    }

    public class AutoEscalationEngine
    {
        private readonly ApplicationDbContext _db;
        public AutoEscalationEngine(ApplicationDbContext db) { _db = db; }

        public async Task<List<DepartmentDelayLog>> GetOpenDelaysAsync(int thresholdMinutes)
        {
            return await _db.Set<DepartmentDelayLog>()
                .Where(d => d.DelayEnd == null && EF.Functions.DateDiffMinute(d.DelayStart, DateTime.UtcNow) > thresholdMinutes)
                .ToListAsync();
        }
        public async Task EscalateDelayAsync(int delayId, int escalationLevel, string reason)
        {
            var delay = await _db.Set<DepartmentDelayLog>().FindAsync(delayId);
            if (delay != null)
            {
                delay.EscalationLevel = escalationLevel;
                delay.EscalationReason = reason;
                await _db.SaveChangesAsync();
            }
        }
    }

    public class InactivityHeatmapAgent
    {
        private readonly ApplicationDbContext _db;
        public InactivityHeatmapAgent(ApplicationDbContext db) { _db = db; }

        public async Task<Dictionary<string, int>> GetDepartmentInactivityAsync()
        {
            // Example: count open delays per department
            return await _db.Set<DepartmentDelayLog>()
                .Where(d => d.DelayEnd == null)
                .GroupBy(d => d.Department)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }

    public class GhostDelayAuditor
    {
        private readonly ApplicationDbContext _db;
        private readonly Random _rand = new();
        public GhostDelayAuditor(ApplicationDbContext db) { _db = db; }

        public async Task<List<DepartmentDelayLog>> SampleCompletedDelaysAsync(int sampleSize = 10)
        {
            var completed = await _db.Set<DepartmentDelayLog>().Where(d => d.DelayEnd != null).ToListAsync();
            return completed.OrderBy(x => _rand.Next()).Take(sampleSize).ToList();
        }
        public bool IsShadowDelay(DepartmentDelayLog log)
        {
            // Example: shadow delay if resolution notes contain 'manual override' or escalation level > 2
            return (log.ResolutionNotes?.ToLower().Contains("manual override") ?? false) || (log.EscalationLevel ?? 0) > 2;
        }
    }
}
