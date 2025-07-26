using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public enum TechnicianAuditActionType
    {
        PunchIn,
        PunchOut,
        Note,
        GPSPing,
        ManualOverride
    }

    public class TechnicianAuditLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public TechnicianAuditActionType ActionType { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Source { get; set; } // Mobile, Dispatcher, GPS
    }
}
