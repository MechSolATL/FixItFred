using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public enum TechnicianAuditActionType
    {
        /// <summary>
        /// Represents a technician punching in.
        /// </summary>
        PunchIn,

        /// <summary>
        /// Represents a technician punching out.
        /// </summary>
        PunchOut,

        /// <summary>
        /// Represents a note added to the audit log.
        /// </summary>
        Note,

        /// <summary>
        /// Represents a GPS ping action.
        /// </summary>
        GPSPing,

        /// <summary>
        /// Represents a manual override action.
        /// </summary>
        ManualOverride
    }

    public class TechnicianAuditLog
    {
        /// <summary>
        /// The unique identifier for the technician audit log entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the audit log entry.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The type of action performed by the technician.
        /// </summary>
        public TechnicianAuditActionType ActionType { get; set; }

        /// <summary>
        /// The timestamp when the action was performed.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Additional notes about the action, if applicable.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The latitude of the technician's location, if applicable.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude of the technician's location, if applicable.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The source of the action (e.g., Mobile, Dispatcher, GPS).
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The route tag associated with the action, if applicable.
        /// </summary>
        public string? RouteTag { get; set; }
    }
}
