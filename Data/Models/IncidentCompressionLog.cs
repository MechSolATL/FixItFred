using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class IncidentCompressionLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ClusterKey { get; set; } = string.Empty; // e.g. address, unit, complaint type
        public int OccurrenceCount { get; set; }
        public string? EquipmentFaults { get; set; }
        public string? DispatchIssues { get; set; }
        public bool TechBurnoutSuspected { get; set; }
        public bool ClientAbuseSuspected { get; set; }
        public string? Notes { get; set; }
    }
}
