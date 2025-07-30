using MVP_Core.Data.Models;
using MVP_Core.Services.Dispatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVP_Core.Services
{
    /// <summary>
    /// Service for scheduling and processing notifications.
    /// </summary>
    public class NotificationSchedulerService
    {
        private readonly ApplicationDbContext _db;
        private readonly NotificationDispatchEngine _dispatchEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSchedulerService"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        /// <param name="dispatchEngine">The notification dispatch engine.</param>
        public NotificationSchedulerService(ApplicationDbContext db, NotificationDispatchEngine dispatchEngine)
        {
            _db = db;
            _dispatchEngine = dispatchEngine;
        }

        /// <summary>
        /// Queues a notification for future dispatch.
        /// </summary>
        /// <param name="recipient">The recipient of the notification.</param>
        /// <param name="channel">The channel type for the notification.</param>
        /// <param name="trigger">The trigger type for the notification.</param>
        /// <param name="targetId">The target ID associated with the notification.</param>
        /// <param name="messageBody">The message body of the notification.</param>
        /// <param name="scheduledTime">The time when the notification is scheduled to be sent.</param>
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

        /// <summary>
        /// Processes the notification queue, sending pending notifications.
        /// </summary>
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

        /// <summary>
        /// Cancels a pending notification.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to cancel.</param>
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
