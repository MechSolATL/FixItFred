using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;

namespace Services.Admin
{
    /// <summary>
    /// Analyzes technician activity logs and auto-flags behavioral anomalies.
    /// </summary>
    public class TechnicianBehaviorAnalyzerService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianBehaviorAnalyzerService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Analyze technician patterns and return detected behavior logs.
        /// </summary>
        public async Task<List<TechnicianBehaviorLog>> AnalyzeTechnicianPatternsAsync()
        {
            var auditLogs = await _db.TechnicianAuditLogs.ToListAsync();
            var behaviorLogs = new List<TechnicianBehaviorLog>();
            var grouped = auditLogs.GroupBy(l => l.TechnicianId);
            foreach (var group in grouped)
            {
                // Detect excessive late PunchIns (after 9 AM)
                var latePunchIns = group.Where(l => l.ActionType == TechnicianAuditActionType.PunchIn && l.Timestamp.Hour > 9).ToList();
                if (latePunchIns.Count > 2)
                {
                    behaviorLogs.Add(new TechnicianBehaviorLog
                    {
                        TechnicianId = group.Key,
                        BehaviorType = "LatePunchIn",
                        Description = $"{latePunchIns.Count} late punch-ins detected.",
                        SeverityLevel = latePunchIns.Count > 5 ? "Critical" : "Warning",
                        Timestamp = DateTime.UtcNow
                    });
                }
                // Detect rapid ManualOverride actions (within 5 minutes)
                var overrides = group.Where(l => l.ActionType == TechnicianAuditActionType.ManualOverride).OrderBy(l => l.Timestamp).ToList();
                for (int i = 1; i < overrides.Count; i++)
                {
                    if ((overrides[i].Timestamp - overrides[i - 1].Timestamp).TotalMinutes < 5)
                    {
                        behaviorLogs.Add(new TechnicianBehaviorLog
                        {
                            TechnicianId = group.Key,
                            BehaviorType = "RapidManualOverride",
                            Description = "Rapid manual override detected.",
                            SeverityLevel = "Warning",
                            Timestamp = overrides[i].Timestamp
                        });
                    }
                }
                // Detect long idle sessions (gap between PunchOut and next PunchIn > 60 min)
                var punchIns = group.Where(l => l.ActionType == TechnicianAuditActionType.PunchIn).OrderBy(l => l.Timestamp).ToList();
                var punchOuts = group.Where(l => l.ActionType == TechnicianAuditActionType.PunchOut).OrderBy(l => l.Timestamp).ToList();
                for (int i = 0; i < punchOuts.Count && i < punchIns.Count - 1; i++)
                {
                    var idleMinutes = (punchIns[i + 1].Timestamp - punchOuts[i].Timestamp).TotalMinutes;
                    if (idleMinutes > 60)
                    {
                        behaviorLogs.Add(new TechnicianBehaviorLog
                        {
                            TechnicianId = group.Key,
                            BehaviorType = "LongIdleSession",
                            Description = $"Idle for {idleMinutes:F0} minutes.",
                            SeverityLevel = idleMinutes > 120 ? "Critical" : "Info",
                            Timestamp = punchOuts[i].Timestamp
                        });
                    }
                }
            }
            return behaviorLogs;
        }

        /// <summary>
        /// Auto-flag critical behaviors and persist to database.
        /// </summary>
        public async Task AutoFlagCriticalBehaviorsAsync()
        {
            var behaviors = await AnalyzeTechnicianPatternsAsync();
            var criticals = behaviors.Where(b => b.SeverityLevel == "Critical").ToList();
            if (criticals.Any())
            {
                await _db.TechnicianBehaviorLogs.AddRangeAsync(criticals);
                await _db.SaveChangesAsync();
            }
        }
    }
}
