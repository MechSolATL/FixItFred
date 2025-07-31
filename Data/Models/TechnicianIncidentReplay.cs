using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class TechnicianIncidentReplay
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string SnapshotHash { get; set; } = string.Empty;
        public string TriggeredBy { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
