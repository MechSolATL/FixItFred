using System;

namespace Data.Models.Reports
{
    public class ReportResult
    {
        public Guid TechnicianId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Summary { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = "Technician_Report.pdf";
    }
}
