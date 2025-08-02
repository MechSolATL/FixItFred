using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Models.ViewModels;
using ViewModels;

namespace Services.Diagnostics
{
    public class DiagnosticsAIService
    {
        private readonly ApplicationDbContext _db;
        public DiagnosticsAIService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Analyze logs and metrics for anomalies and return latest alerts
        public async Task<List<DiagnosticsAlertResult>> GetLatestAlertsAsync(int count = 3)
        {
            // Simulate: In real use, analyze logs/metrics for anomalies
            var now = DateTime.UtcNow;
            var alerts = new List<DiagnosticsAlertResult>();

            // Example: CPU spike
            var cpuSpike = _db.SystemDiagnosticLogs
                .Where(l => l.Summary.Contains("CPU spike") || l.Details.Contains("CPU spike"))
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();
            if (cpuSpike != null)
            {
                alerts.Add(new DiagnosticsAlertResult
                {
                    Type = "CPU Spike",
                    SeverityLevel = "Critical",
                    Description = cpuSpike.Summary,
                    Timestamp = cpuSpike.Timestamp,
                    SuggestedFix = "Scale up server resources or investigate running processes."
                });
            }

            // Example: Thread stall
            var threadStall = _db.SystemDiagnosticLogs
                .Where(l => l.Summary.Contains("thread stall") || l.Details.Contains("thread stall"))
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();
            if (threadStall != null)
            {
                alerts.Add(new DiagnosticsAlertResult
                {
                    Type = "Thread Stall",
                    SeverityLevel = "Warning",
                    Description = threadStall.Summary,
                    Timestamp = threadStall.Timestamp,
                    SuggestedFix = "Check for deadlocks or thread pool exhaustion."
                });
            }

            // Example: Failed job
            var failedJob = _db.SystemDiagnosticLogs
                .Where(l => l.Summary.Contains("failed job") || l.Details.Contains("failed job"))
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();
            if (failedJob != null)
            {
                alerts.Add(new DiagnosticsAlertResult
                {
                    Type = "Failed Job",
                    SeverityLevel = "Error",
                    Description = failedJob.Summary,
                    Timestamp = failedJob.Timestamp,
                    SuggestedFix = "Review job logs and retry or fix the underlying issue."
                });
            }

            // Example: Downtime (simulate by recent inactive heartbeat)
            var recentInactive = _db.UptimeHeartbeatLogs
                .OrderByDescending(l => l.HeartbeatAt)
                .FirstOrDefault(l => !l.IsActive);
            if (recentInactive != null)
            {
                alerts.Add(new DiagnosticsAlertResult
                {
                    Type = "Downtime",
                    SeverityLevel = "Critical",
                    Description = "Subsystem downtime detected.",
                    Timestamp = recentInactive.HeartbeatAt,
                    SuggestedFix = "Check server/network connectivity and restart affected services."
                });
            }

            // Example: Delayed email dispatch
            var delayedEmail = _db.SystemDiagnosticLogs
                .Where(l => l.Summary.Contains("email delay") || l.Details.Contains("email delay"))
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();
            if (delayedEmail != null)
            {
                alerts.Add(new DiagnosticsAlertResult
                {
                    Type = "Email Delay",
                    SeverityLevel = "Warning",
                    Description = delayedEmail.Summary,
                    Timestamp = delayedEmail.Timestamp,
                    SuggestedFix = "Check email queue and SMTP server status."
                });
            }

            // Return most recent N alerts
            return alerts.OrderByDescending(a => a.Timestamp).Take(count).ToList();
        }
    }
}
