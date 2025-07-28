// Generated for Service-Atlanta.com - Service Request Dashboard

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

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

        // Encrypted fields
        public string OldValueEncrypted { get; set; } = string.Empty;
        public string NewValueEncrypted { get; set; } = string.Empty;
        public string IPAddressEncrypted { get; set; } = string.Empty;

        // Integrity hash
        public string IntegrityHash { get; set; } = string.Empty;

        /// <summary>
        /// Security level of the audit log entry (e.g., Standard, High, Critical).
        /// </summary>
        public string SecurityLevel { get; set; } = "Standard";

        /// <summary>
        /// UTC timestamp when the change was logged.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // [NotMapped] decrypted accessors
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string OldValue { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string NewValue { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string IPAddress { get; set; } = string.Empty;
    }
}
