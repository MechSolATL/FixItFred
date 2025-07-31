// Restored by FixItFred per Nova Directive — Sprint 70.4
namespace Data.Models;
public class RootCauseCorrelationLog
{
    public int Id { get; set; }
    public string RootCauseLabel { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public DateTime LoggedAt { get; set; }
    public string AffectedModule { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
