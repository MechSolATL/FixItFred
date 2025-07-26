public class SystemSnapshotLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SnapshotType { get; set; }
    public string Summary { get; set; }
    public string DetailsJson { get; set; }
    public string CreatedBy { get; set; }
    public string SnapshotHash { get; set; } // Added for replay selection
}
// ...existing code...