namespace MVP_Core.Models.Admin
{
    public class DispatcherAuditLog
    {
        public int Id { get; set; }
        public string ActionType { get; set; } // Reassign, Escalate, Cancel, etc.
        public int RequestId { get; set; }
        public int? TechnicianId { get; set; }
        public string? Notes { get; set; }
        public string PerformedBy { get; set; }
        public string PerformedByRole { get; set; } // New property for role tracking
        public DateTime Timestamp { get; set; }
    }
}
