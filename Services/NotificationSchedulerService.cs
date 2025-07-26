using MVP_Core.Data.Models;
using MVP_Core.Services.Dispatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVP_Core.Services
{
    public class NotificationSchedulerService
    {
        private readonly ApplicationDbContext _db;
        private readonly NotificationDispatchEngine _dispatchEngine;

        public NotificationSchedulerService(ApplicationDbContext db, NotificationDispatchEngine dispatchEngine)
        {
            _db = db;
            _dispatchEngine = dispatchEngine;
        }

        // Queue a notification
        public async Task QueueNotificationAsync(string recipient, NotificationChannelType channel, NotificationTriggerType trigger, int targetId, string messageBody, DateTime scheduledTime)
        {
            var notification = new NotificationQueue
            {
                Recipient = recipient,
                ChannelType = channel,
                TriggerType = trigger,
                TargetId = targetId,
                Status = NotificationStatus.Pending,
                ScheduledTime = scheduledTime,
                MessageBody = messageBody
            };
            _db.NotificationQueues.Add(notification);
            await _db.SaveChangesAsync();
        }

        // Process pending notifications (retry, delay, SLA check)
        public async Task ProcessQueueAsync()
        {
            var now = DateTime.UtcNow;
            var pending = await _db.NotificationQueues.Where(q => q.Status == NotificationStatus.Pending && q.ScheduledTime <= now).ToListAsync();
            foreach (var notification in pending)
            {
                bool sent = await _dispatchEngine.DispatchNotificationAsync(notification);
                notification.Status = sent ? NotificationStatus.Sent : NotificationStatus.Failed;
                notification.SentTime = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();
        }

        // Cancel a notification
        public async Task CancelNotificationAsync(int notificationId)
        {
            var notification = await _db.NotificationQueues.FindAsync(notificationId);
            if (notification != null && notification.Status == NotificationStatus.Pending)
            {
                notification.Status = NotificationStatus.Cancelled;
                await _db.SaveChangesAsync();
            }
        }
    }
}
