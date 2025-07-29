using System;

namespace MVP_Core.Data.Models.PatchAnalytics
{
    public class PromoImpactScore
    {
        public int Id { get; set; }
        public string PromoTag { get; set; } = string.Empty;
        public int MentionCount { get; set; }
        public int? TechnicianId { get; set; }
        public DateTime LastMentioned { get; set; }
    }
}
