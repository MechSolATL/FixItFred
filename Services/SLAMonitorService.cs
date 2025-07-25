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
    private readonly IServiceProvider _serviceProvider;
    private readonly NotificationDispatchEngine _notificationDispatchEngine;
    private readonly DispatcherService _dispatcherService;
    private readonly bool _enableFallback;

    public SLAMonitorService(IServiceProvider serviceProvider, NotificationDispatchEngine notificationDispatchEngine, DispatcherService dispatcherService, bool enableFallback = true)
    {
        _serviceProvider = serviceProvider;
        _notificationDispatchEngine = notificationDispatchEngine;
        _dispatcherService = dispatcherService;
        _enableFallback = enableFallback;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var now = DateTime.UtcNow;
                var candidates = db.ScheduleQueues
                    .Where(q => (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched)
                        && q.SLAExpiresAt < now)
                    .ToList();
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
                            entry.AssignedTechnicianName = backupTech.FullName;
                            entry.EstimatedArrival = _dispatcherService.PredictETA(backupTech, entry.Zone, 0);
                            entry.Status = ScheduleStatus.Dispatched;
                            db.EscalationLogs.Add(new EscalationLogEntry {
                                ScheduleQueueId = entry.Id,
                                TriggeredBy = "system",
                                Reason = "Automatic SLA Escalation Fallback",
                                ActionTaken = $"Reassigned to backup tech {backupTech.FullName}",
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
