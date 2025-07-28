using System;

namespace MVP_Core.Models
{
    public enum ViolationPatternType
    {
        LateStartStreak,
        MediaGaps,
        FakeLocationLoop,
        FrequentBackdating,
        GpsMismatch,
        ConsecutiveMissingUploads
    }

    public class ViolationInsightModel
    {
        public int TechnicianId { get; set; }
        public ViolationPatternType PatternType { get; set; }
        public double ConfidenceScore { get; set; }
        public string SummaryText { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
