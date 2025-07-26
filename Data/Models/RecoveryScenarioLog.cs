public class RecoveryScenarioLog
{
    public int Id { get; set; }
    public string ScenarioName { get; set; }
    public string TriggeredBy { get; set; }
    public DateTime ScheduledForUtc { get; set; }
    public bool Executed { get; set; }
    public DateTime? ExecutedAtUtc { get; set; }
    public string? OutcomeSummary { get; set; }
    public string? SnapshotHash { get; set; }
    public string? Notes { get; set; }
}