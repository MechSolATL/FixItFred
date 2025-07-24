// Generated for Service-Atlanta.com - Service Request Dashboard

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents an entry in the audit log table for tracking user changes and actions.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the user responsible for the change. Use 0 if anonymous or system-triggered.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Describes the type of change (e.g., Update, Create, Delete).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ChangeType { get; set; } = string.Empty;

        /// <summary>
        /// Value before the change occurred.
        /// </summary>
        public string OldValue { get; set; } = string.Empty;

        /// <summary>
        /// Value after the change occurred.
        /// </summary>
        public string NewValue { get; set; } = string.Empty;

        /// <summary>
        /// IP address of the client who made the request.
        /// </summary>
        public string IPAddress { get; set; } = "Unknown";

        /// <summary>
        /// UTC timestamp when the change was logged.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
