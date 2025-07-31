using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;

namespace Services.Diagnostics
{
    public class AIWarningQueueEngine
    {
        private readonly ApplicationDbContext _db;
        public AIWarningQueueEngine(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<ComplianceAlertLog> GetAIWarnings()
        {
            // For demo: return all unacknowledged alerts with severity 'AI'
            return _db.ComplianceAlertLogs
                .Where(a => a.Severity == "AI" && !a.IsAcknowledged)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }
    }
}
