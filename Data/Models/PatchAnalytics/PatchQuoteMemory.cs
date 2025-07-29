using System;

namespace MVP_Core.Data.Models.PatchAnalytics
{
    public class PatchQuoteMemory
    {
        public int Id { get; set; }
        public string Quote { get; set; } = string.Empty;
        public int? TriggeredByTechnicianId { get; set; }
        public int UsageCount { get; set; }
        public DateTime LastUsed { get; set; }
    }
}
