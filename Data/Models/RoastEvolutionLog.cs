using System;

namespace MVP_Core.Data.Models
{
    public class RoastEvolutionLog
    {
        public int Id { get; set; }
        public int RoastTemplateId { get; set; }
        public string EditType { get; set; } // e.g., 'Promote', 'Retire', 'Edit', 'AISeed', 'LegacyTag'
        public string Editor { get; set; } // User or system
        public DateTime Timestamp { get; set; }
        public string Notes { get; set; } // Details about the change
        public double EffectivenessScore { get; set; } // e.g., post-change success rate
        public bool Promoted { get; set; }
        public bool Retired { get; set; }
        public bool IsLegacy { get; set; }
        public bool IsAIAuthored { get; set; }
        public string PreviousMessage { get; set; }
        public string NewMessage { get; set; }
    }
}
