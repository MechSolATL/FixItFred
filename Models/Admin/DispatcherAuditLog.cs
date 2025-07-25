namespace MVP_Core.Models.Admin
{
    public class DispatcherAuditLog
    {
        public int Id { get; set; }
        public required string ActionType { get; set; }
        public int RequestId { get; set; }
        public int? TechnicianId { get; set; }
        public required string PerformedBy { get; set; }
        public required string PerformedByRole { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
    }
}
