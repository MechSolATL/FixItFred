// Sprint 85.5 — Trust Incident Timeline
using System;

namespace MVP_Core.Models
{
    // Sprint 85.5 — Trust Incident Timeline
    public class TrustTimelineEntry
    {
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public int LinkedLogId { get; set; }
    }
}
