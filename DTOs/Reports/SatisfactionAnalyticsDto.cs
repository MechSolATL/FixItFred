// FixItFred – Sprint 44 Build Restoration
namespace MVP_Core.DTOs.Reports
{
    /// <summary>
    /// DTO for satisfaction analytics summaries.
    /// </summary>
    public class SatisfactionAnalyticsDto
    {
        public string GroupKey { get; set; } = string.Empty;
        public string GroupType { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public int Count { get; set; }
    }
}
