// Restored by FixItFred per Nova Directive — Sprint 70.4
namespace MVP_Core.Data.Models;
public class SlaDriftSnapshot
{
    public int Id { get; set; }
    public DateTime SnapshotDate { get; set; }
    public double AverageDriftMinutes { get; set; }
    public int ViolatedCount { get; set; }
    public int TotalJobs { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
