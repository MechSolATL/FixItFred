// Sprint92_Fix_GroupB — Corrected syntax & namespace for report model

using System;

namespace Data.Models.Reports
{
    public class ReportResult
    {
        public Guid TechnicianId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Summary { get; set; }
    }
}