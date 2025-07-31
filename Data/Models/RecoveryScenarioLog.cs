using Data.Models;

public class RecoveryScenarioLog
{
    public int Id { get; set; }
    public string ScenarioName { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string TriggeredBy { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime ScheduledForUtc { get; set; }
    public bool Executed { get; set; }
    public DateTime? ExecutedAtUtc { get; set; }
    public string? OutcomeSummary { get; set; }
    public string? SnapshotHash { get; set; }
    public string? Notes { get; set; }
    // Sprint 70.3 Patch - Establish link to originating ServiceRequest
    public int? ServiceRequestId { get; set; }
    public ServiceRequest? ServiceRequest { get; set; }
}