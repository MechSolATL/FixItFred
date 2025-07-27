// Sprint 26.6 Patch Log: CS8601/CS8602/CS8629 fixes — Added null checks and .GetValueOrDefault() for nullable value types. Ensured safe navigation for all nullable references. Previous comments preserved below.
// Sprint 83.7-Hardening: Fixed scoped DbContext usage in hosted service
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using MVP_Core.Services.Dispatch;

// Sprint 34.3 - SLA Monitoring Loop
public class SLAMonitorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NotificationDispatchEngine _notificationDispatchEngine;
    private readonly DispatcherService _dispatcherService;
    private readonly bool _enableFallback;

    public SLAMonitorService(IServiceScopeFactory scopeFactory, NotificationDispatchEngine notificationDispatchEngine, DispatcherService dispatcherService, bool enableFallback = true)
    {
        _scopeFactory = scopeFactory;
        _notificationDispatchEngine = notificationDispatchEngine;
        _dispatcherService = dispatcherService;
        _enableFallback = enableFallback;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var now = DateTime.UtcNow;
                var candidates = db.ScheduleQueues
                    .Where(q => (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched)
                        && q.SLAExpiresAt != null && q.SLAExpiresAt < now)
                    .ToList(); // Fix: Added null check for SLAExpiresAt
                foreach (var entry in candidates)
                {
                    bool alreadyEscalated = db.EscalationLogs.Any(e => e.ScheduleQueueId == entry.Id);
                    if (alreadyEscalated) continue;
                    // Sprint 34.3 - Auto Broadcast SLA Violation
                    await _notificationDispatchEngine.BroadcastSLAEscalationAsync(entry.Zone, $"[Auto SLA Escalation] Job #{entry.ServiceRequestId} in {entry.Zone}");
                    // Sprint 34.3 - Log to EscalationLogEntry
                    db.EscalationLogs.Add(new EscalationLogEntry {
                        ScheduleQueueId = entry.Id,
                        TriggeredBy = "system",
                        Reason = "Automatic SLA Escalation",
                        ActionTaken = "None – pending manual review",
                        CreatedAt = now
                    });
                    // Sprint 34.3 - Auto Technician Reassignment on SLA
                    if (_enableFallback)
                    {
                        var backupTech = _dispatcherService.FindAvailableTechnicianForZone(entry.Zone);
                        if (backupTech != null && backupTech.Id != entry.TechnicianId)
                        {
                            entry.TechnicianId = backupTech.Id;
                            entry.AssignedTechnicianName = backupTech.FullName ?? string.Empty;
                            var techStatus = new MVP_Core.Models.Admin.TechnicianStatusDto {
                                TechnicianId = backupTech.Id,
                                Name = backupTech.FullName ?? string.Empty,
                                Status = backupTech.Specialty ?? string.Empty,
                                DispatchScore = 100,
                                LastPing = DateTime.UtcNow,
                                AssignedJobs = 0,
                                LastUpdate = DateTime.UtcNow
                            };
                            entry.EstimatedArrival = _dispatcherService.PredictETA(techStatus, entry.Zone, 0).GetAwaiter().GetResult();
                            entry.Status = ScheduleStatus.Dispatched;
                            db.EscalationLogs.Add(new EscalationLogEntry {
                                ScheduleQueueId = entry.Id,
                                TriggeredBy = "system",
                                Reason = "Automatic SLA Escalation Fallback",
                                ActionTaken = $"Reassigned to backup tech {backupTech.FullName ?? "Unknown"}",
                                CreatedAt = now
                            });
                        }
                    }
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
