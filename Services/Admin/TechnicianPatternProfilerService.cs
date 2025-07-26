using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class TechnicianPatternProfilerService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianPatternProfilerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TechnicianPatternProfile>> AnalyzeEscalationPatternsAsync()
        {
            var since = DateTime.UtcNow.AddDays(-60);
            var logs = await _db.TechnicianEscalationLogs
                .Where(e => e.CreatedAt >= since)
                .ToListAsync();

            var grouped = logs.GroupBy(e => e.TechnicianId);
            var result = new List<TechnicianPatternProfile>();

            foreach (var group in grouped)
            {
                var count = group.Count();
                string riskLevel = count > 10 ? "High" : count > 5 ? "Medium" : "Low";
                string patternType = count > 10 ? "Escalation Spike" : "Normal";
                result.Add(new TechnicianPatternProfile
                {
                    TechnicianId = group.Key,
                    PatternType = patternType,
                    Description = $"{count} escalations in last 60 days.",
                    DetectedAt = DateTime.UtcNow,
                    RiskLevel = riskLevel,
                    IsAutoFlagged = riskLevel == "High"
                });
            }
            return result;
        }
    }
}
