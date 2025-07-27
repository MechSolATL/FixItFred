public class ReplayAuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SnapshotHash { get; set; } = string.Empty; // Sprint 80: Initialized non-nullable properties in ReplayAuditLog
    public string TriggeredBy { get; set; } = string.Empty; // Sprint 80: Initialized non-nullable properties in ReplayAuditLog
    public bool Success { get; set; }
    public string Notes { get; set; } = string.Empty; // Sprint 80: Initialized non-nullable properties in ReplayAuditLog

    // Sprint 80: Initialized non-nullable properties in ReplayAuditLog
    public ReplayAuditLog()
    {
        SnapshotHash = string.Empty;
        TriggeredBy = string.Empty;
        Notes = string.Empty;
    }
}