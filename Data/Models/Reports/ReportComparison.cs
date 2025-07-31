using System;

namespace Data.Models.Reports
{
    public class ReportComparison
    {
        public Guid[] TechnicianIds { get; set; } = Array.Empty<Guid>();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ComparisonData { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = "Comparison_Report.pdf";
    }
}
