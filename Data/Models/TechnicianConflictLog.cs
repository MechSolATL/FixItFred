using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TechnicianConflictLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int ConflictWithId { get; set; }
        public string ConflictType { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public bool Resolved { get; set; } = false;
        public string ResolutionNotes { get; set; } = string.Empty;
        public int SeverityLevel { get; set; } = 1;
        public double TrustImpactScore { get; set; } = 0.0;
    }
}
