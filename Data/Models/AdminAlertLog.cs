// Restored by FixItFred per Nova Directive — Sprint 70.4
namespace MVP_Core.Data.Models;
public class AdminAlertLog
{
    public int Id { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Severity { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
