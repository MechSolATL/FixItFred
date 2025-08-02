using System;
using System.Threading.Tasks;
using Data.Models;

namespace Services
{
    /// <summary>
    /// Interface for notification scheduling service.
    /// [FixItFredComment:Sprint1004 - DI registration verified] Created interface for proper DI registration
    /// </summary>
    public interface INotificationSchedulerService
    {
        /// <summary>
        /// Queues a notification for future dispatch.
        /// </summary>
        /// <param name="recipient">The recipient of the notification.</param>
        /// <param name="channel">The channel type for the notification.</param>
        /// <param name="trigger">The trigger type for the notification.</param>
        /// <param name="targetId">The target ID associated with the notification.</param>
        /// <param name="messageBody">The message body of the notification.</param>
        /// <param name="scheduledTime">The time when the notification is scheduled to be sent.</param>
        Task QueueNotificationAsync(string recipient, NotificationChannelType channel, NotificationTriggerType trigger, int targetId, string messageBody, DateTime scheduledTime);

        /// <summary>
        /// Processes the notification queue, sending pending notifications.
        /// </summary>
        Task ProcessQueueAsync();

        /// <summary>
        /// Cancels a pending notification.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to cancel.</param>
        Task CancelNotificationAsync(int notificationId);
    }
}