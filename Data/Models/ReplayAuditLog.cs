public class ReplayAuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SnapshotHash { get; set; }
    public string TriggeredBy { get; set; }
    public bool Success { get; set; }
    public string Notes { get; set; }
}
// ...existing code...