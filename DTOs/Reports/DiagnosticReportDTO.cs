namespace DTOs.Reports
{
    public class DiagnosticReportDTO
    {
        public string Status { get; set; } = "OK";
        public int ErrorCount { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}
