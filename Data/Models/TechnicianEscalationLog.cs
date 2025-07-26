public class TechnicianEscalationLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int EscalationLevel { get; set; }
    public string EscalationReason { get; set; }
    public bool Resolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string ResolutionNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
