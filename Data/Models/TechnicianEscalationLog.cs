public class TechnicianEscalationLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int EscalationLevel { get; set; }
    public string EscalationReason { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public bool Resolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string ResolutionNotes { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime CreatedAt { get; set; }
}
