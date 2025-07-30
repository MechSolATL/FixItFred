public class TechnicianEscalationLog
{
    /// <summary>
    /// The unique identifier for the technician escalation log entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the technician associated with the escalation.
    /// </summary>
    public int TechnicianId { get; set; }

    /// <summary>
    /// The level of escalation.
    /// </summary>
    public int EscalationLevel { get; set; }

    /// <summary>
    /// The reason for the escalation.
    /// </summary>
    public string EscalationReason { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning

    /// <summary>
    /// Indicates whether the escalation has been resolved.
    /// </summary>
    public bool Resolved { get; set; }

    /// <summary>
    /// The timestamp when the escalation was resolved, if applicable.
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Notes about the resolution of the escalation.
    /// </summary>
    public string ResolutionNotes { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning

    /// <summary>
    /// The timestamp when the escalation log entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
