using System;

namespace Data.Models
{
    // Sprint 91.23 - Technician Report Library
    public class TechnicianReport
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int? ServiceRequestId { get; set; }
        public string Title { get; set; }
        public string WorkSummary { get; set; }
        public string AIAnalysis { get; set; }
        public string Recommendations { get; set; }
        public DateTime SubmittedOn { get; set; }
        public bool IsFinalized { get; set; }
    }
}
