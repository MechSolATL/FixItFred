using System;
using MVP_Core.Data.Models;

namespace MVP_Core.Data.Models
{
    public class SlaMissAlert
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime ScheduledArrival { get; set; }
        public DateTime ActualArrival { get; set; }
        public int ServiceRequestId { get; set; }
        public bool SlaViolated { get; set; }
        public int ViolationMinutes { get; set; }
        public bool AutoFlagged { get; set; }
        public DateTime? AlertSentAt { get; set; }
        public string ResolutionStatus { get; set; } = string.Empty;
        public Technician? Technician { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
    }
}