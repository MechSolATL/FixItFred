// FixItFred: Sprint 30B - Real-Time Dispatch
// Created: 2025-07-25
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubs;
using Services.FollowUp;
using Data;
using Data.Models;

namespace Services.Dispatch
{
    public class NotificationDispatchEngine
    {
        private readonly IHubContext<ETAHub> _hubContext;
        private readonly ApplicationDbContext _db;
        private readonly INotificationHelperService _notificationHelper;

        public NotificationDispatchEngine(IHubContext<ETAHub> hubContext, ApplicationDbContext db, INotificationHelperService notificationHelper)
        {
            _hubContext = hubContext;
            _db = db;
            _notificationHelper = notificationHelper;
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
                name,
                lat,
                lng,
                status,
                jobs,
                eta,
                currentJob
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
                await _notificationHelper.SendFollowUpNotificationAsync(_db, job.ServiceRequestId, "MissedConfirmation", "Gentle");
            foreach (var job in unassignedJobs)
                await _notificationHelper.SendFollowUpNotificationAsync(_db, job.ServiceRequestId, "UnassignedJob", "Gentle");
            foreach (var job in repeatedSLABreaches)
                await _notificationHelper.SendFollowUpNotificationAsync(_db, job.ServiceRequestId, "RepeatedSLABreach", "Assertive");
            foreach (var req in failedContacts)
                await _notificationHelper.SendFollowUpNotificationAsync(_db, req.Id, "FailedContact", "Gentle");
        }

        // Sprint 52.0: Send follow-up notification and log action
        public async Task SendFollowUpNotificationAsync(int serviceRequestId, string reason, string escalationLevel)
        {
            await _notificationHelper.SendFollowUpNotificationAsync(_db, serviceRequestId, reason, escalationLevel);
        }

        // Sprint 55.0: Dispatch notification from NotificationQueue
        public async Task<bool> DispatchNotificationAsync(NotificationQueue notification)
        {
            try
            {
                switch (notification.ChannelType)
                {
                    case NotificationChannelType.Email:
                        // TODO: Integrate with EmailService
                        // await _emailService.SendEmailAsync(notification.Recipient, "Notification", notification.MessageBody);
                        break;
                    case NotificationChannelType.SMS:
                        // TODO: Integrate with SmsService (Twilio)
                        // await _smsService.SendSmsAsync(notification.Recipient, notification.MessageBody);
                        break;
                    case NotificationChannelType.Push:
                        // TODO: Integrate with WebPush
                        // await _webPushService.SendPushAsync(notification.Recipient, notification.MessageBody);
                        break;
                }
                // Optionally broadcast via SignalR for live status
                if (notification.TriggerType == NotificationTriggerType.OnTheWay)
                {
                    await _hubContext.Clients.All.SendAsync("TechOnTheWay", notification.Recipient, notification.MessageBody);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Sprint 55.0: Estimate notification triggers
        public void TriggerEstimateAcknowledged(string customerEmail, Guid estimateId)
        {
            // Example logic: log, send email, or push notification
            var estimate = _db.BillingInvoiceRecords.FirstOrDefault(e => e.Id == estimateId);
            if (estimate != null)
            {
                // TODO: Integrate with EmailService/SMS/WebPush
                // For now, just log or update status
                estimate.IsAcknowledged = true;
                estimate.AcknowledgedBy = customerEmail;
                estimate.AcknowledgedDate = DateTime.UtcNow;
                _db.SaveChanges();
            }
        }

        public void TriggerEstimateDecision(string customerEmail, Guid estimateId, bool wasAccepted)
        {
            var estimate = _db.BillingInvoiceRecords.FirstOrDefault(e => e.Id == estimateId);
            if (estimate != null)
            {
                estimate.WasAccepted = wasAccepted;
                estimate.DecisionDate = DateTime.UtcNow;
                // TODO: Integrate with EmailService/SMS/WebPush
                _db.SaveChanges();
            }
        }
    }
}
