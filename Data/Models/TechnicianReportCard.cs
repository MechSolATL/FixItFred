using System;

namespace Data.Models
{
    public class TechnicianReportCard
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public int LateClockInCount { get; set; }
        public int SevereLateClockInCount { get; set; }
        public int IdleMinutesWeek { get; set; }
        public double EgoInfluenceScore { get; set; }
        public double ConfidenceShift { get; set; }
        public double PulseScore { get; set; }
        public double TrustScore { get; set; }
        public string? Notes { get; set; }
    }
}
