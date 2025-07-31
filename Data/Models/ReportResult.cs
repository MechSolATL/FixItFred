namespace Data.Models
{
    public class ReportResult
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = "Technician_Report.pdf";
        public Guid TechnicianId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}
