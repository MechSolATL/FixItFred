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
    }
}
