// FixItFred: Sprint 30B - Real-Time Dispatch
// Created: 2025-07-25
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Services.FollowUp;

namespace MVP_Core.Services.Dispatch
{
    public class NotificationDispatchEngine
    {
        private readonly IHubContext<ETAHub> _hubContext;
        private readonly ApplicationDbContext _db;
        private readonly FollowUpAIService _followUpAIService;

        public NotificationDispatchEngine(IHubContext<ETAHub> hubContext, ApplicationDbContext db, FollowUpAIService followUpAIService)
        {
            _hubContext = hubContext;
            _db = db;
            _followUpAIService = followUpAIService;
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

        // Sprint 50.1: Notification Sync & AI Follow-Up
        public async Task SyncAndTriggerFollowUpsAsync()
        {
            var now = DateTime.UtcNow;
            var missedConfirmations = _db.ScheduleQueues.Where(q => !q.DispatcherOverride && q.Status == ScheduleStatus.Pending && q.SLAWindowEnd < now).ToList();
            var unassignedJobs = _db.ScheduleQueues.Where(q => q.TechnicianId == 0 && q.Status == ScheduleStatus.Pending).ToList();
            var repeatedSLABreaches = _db.ScheduleQueues.Where(q => q.Status == ScheduleStatus.Pending && q.SLAExpiresAt < now && q.IsUrgent).ToList();
            var failedContacts = _db.ServiceRequests.Where(r => r.Status == "NoShow" || r.Status == "FailedContact").ToList();

            foreach (var job in missedConfirmations)
                await _followUpAIService.TriggerFollowUp(job.ServiceRequestId, "MissedConfirmation");
            foreach (var job in unassignedJobs)
                await _followUpAIService.TriggerFollowUp(job.ServiceRequestId, "UnassignedJob");
            foreach (var job in repeatedSLABreaches)
                await _followUpAIService.TriggerFollowUp(job.ServiceRequestId, "RepeatedSLABreach");
            foreach (var req in failedContacts)
                await _followUpAIService.TriggerFollowUp(req.Id, "FailedContact");
        }
    }
}
