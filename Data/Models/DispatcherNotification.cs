// Sprint83.4-FinalFixSweep2
namespace MVP_Core.Data.Models
{
    public class DispatcherNotification
    {
        /// <summary>
        /// The unique identifier for the dispatcher notification.
        /// </summary>
        public int Id { get; set; } // Sprint83.4-FinalFixSweep2

        /// <summary>
        /// The user or system that sent the notification.
        /// </summary>
        public string SentBy { get; set; } = string.Empty; // Sprint83.4-FinalFixSweep2

        /// <summary>
        /// The timestamp when the notification was sent.
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow; // Sprint83.4-FinalFixSweep2

        /// <summary>
        /// The type of notification.
        /// </summary>
        public string Type { get; set; } = string.Empty; // Sprint83.4-FinalFixSweep2

        /// <summary>
        /// The message content of the notification.
        /// </summary>
        public string Message { get; set; } = string.Empty; // Sprint83.4-FinalFixSweep2
    }
}