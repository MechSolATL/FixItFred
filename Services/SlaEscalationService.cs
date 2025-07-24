using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System.Linq;
using MVP_Core.Services;

namespace MVP_Core.Services
{
    public class SlaEscalationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<RequestHub> _hubContext;
        public SlaEscalationService(IServiceProvider serviceProvider, IHubContext<RequestHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var notification = scope.ServiceProvider.GetService<INotificationService>();
                    var now = DateTime.UtcNow;
                    var overdue = db.ServiceRequests.Where(r => r.DueDate != null && r.DueDate < now && r.Status != "Complete" && !r.IsEscalated).ToList();
                    foreach (var req in overdue)
                    {
                        // Escalate if not already escalated/logged
                        if (!db.KanbanHistoryLogs.Any(l => l.ServiceRequestId == req.Id && l.ToStatus == "Overdue"))
                        {
                            db.KanbanHistoryLogs.Add(new KanbanHistoryLog
                            {
                                ServiceRequestId = req.Id,
                                FromStatus = req.Status,
                                ToStatus = "Overdue",
                                ChangedBy = "SLA Bot",
                                ChangedAt = now
                            });
                        }
                        req.Status = "Overdue";
                        req.EscalatedAt = now;
                        req.IsEscalated = true;
                        notification?.SendEscalationAlert(req); // stub
                    }
                    if (overdue.Any())
                    {
                        await db.SaveChangesAsync(stoppingToken);
                        await _hubContext.Clients.All.SendAsync("slaUpdated");
                        await _hubContext.Clients.All.SendAsync("EscalationTriggered");
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
