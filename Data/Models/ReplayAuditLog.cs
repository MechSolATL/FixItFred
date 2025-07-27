public class ReplayAuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SnapshotHash { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string TriggeredBy { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public bool Success { get; set; }
    public string Notes { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
}
// ...existing code...