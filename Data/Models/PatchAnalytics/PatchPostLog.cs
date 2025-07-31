using System;

namespace Data.Models.PatchAnalytics
{
    public class PatchPostLog
    {
        public int Id { get; set; }
        public string ContentType { get; set; } = string.Empty; // "ShoutOut", "Promo", "Milestone", etc.
        public string CaptionUsed { get; set; } = string.Empty;
        public string[] Hashtags { get; set; } = Array.Empty<string>();
        public int? TechnicianId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
