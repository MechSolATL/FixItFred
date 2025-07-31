using System;

namespace Data.Models
{
    public class RoastEvolutionLog
    {
        public int Id { get; set; }
        public int RoastTemplateId { get; set; }
        public string EditType { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public string Editor { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public DateTime Timestamp { get; set; }
        public string Notes { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public double EffectivenessScore { get; set; } // e.g., post-change success rate
        public bool Promoted { get; set; }
        public bool Retired { get; set; }
        public bool IsLegacy { get; set; }
        public bool IsAIAuthored { get; set; }
        public string PreviousMessage { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public string NewMessage { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    }
}
