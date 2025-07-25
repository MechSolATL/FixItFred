// FixItFred: Sprint 30B - Real-Time Dispatch
// Created: 2025-07-25
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Dispatch
{
    public class NotificationDispatchEngine
    {
        private readonly IHubContext<ETAHub> _hubContext;
        private readonly ApplicationDbContext _db;

        public NotificationDispatchEngine(IHubContext<ETAHub> hubContext, ApplicationDbContext db)
        {
            _hubContext = hubContext;
            _db = db;
        }

        public async Task BroadcastETAAsync(string zone, string message)
        {
            await _hubContext.Clients.Group($"Zone-{zone}")
                .SendAsync("ReceiveETA", zone, message);

            _db.NotificationsSent.Add(new NotificationsSent
            {
                TechnicianId = 0, // FixItFred: Set actual technician ID if available
                Zone = $"Zone-{zone}",
                Status = message,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        public IEnumerable<ScheduleQueue> GetPendingDispatches()
        {
            return _db.ScheduleQueues
                .Where(q => q.Status.ToString() == "Pending")
                .OrderBy(q => q.ScheduledTime)
                .ToList();
        }

        // Broadcast live technician location/status update
        public async Task BroadcastTechnicianLocationAsync(int technicianId, double lat, double lng, string status, int jobs, string eta, string currentJob, string name)
        {
            await _hubContext.Clients.All.SendAsync("UpdateTechnicianLocation", new {
                id = technicianId,
                name = name,
                lat = lat,
                lng = lng,
                status = status,
                jobs = jobs,
                eta = eta,
                currentJob = currentJob
            });
        }

        // FixItFred: Sprint 34.1 - SLA Escalation Broadcast [2024-07-25T09:45Z]
        public async Task BroadcastSLAEscalationAsync(string zone, string message)
        {
            await _hubContext.Clients.Group($"Zone-{zone}")
                .SendAsync("ReceiveSLAEscalation", message);

            _db.NotificationsSent.Add(new NotificationsSent
            {
                TechnicianId = 0,
                Zone = $"Zone-{zone}",
                Status = message,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }
    }
}
