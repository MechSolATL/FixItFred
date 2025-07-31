using System;

namespace Data.Models
{
    // Sprint 91.23 - Technician Report Library
    public class TechnicianReportFeedback
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int ReviewerId { get; set; }
        public string FeedbackNotes { get; set; }
        public DateTime ReviewedOn { get; set; }
    }
}
