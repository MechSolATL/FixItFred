public class TechnicianScheduleConflictLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int JobId { get; set; }
    public DateTime ConflictDetectedAt { get; set; }
    public string ConflictType { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string Details { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public bool IsAcknowledged { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionSuggestion { get; set; }
    public string? ManualOverrideNote { get; set; }
}
