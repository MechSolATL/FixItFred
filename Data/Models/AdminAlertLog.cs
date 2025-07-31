// Restored by FixItFred per Nova Directive — Sprint 70.4
namespace Data.Models;
public class AdminAlertLog
{
    /// <summary>
    /// The unique identifier for the admin alert log.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The type of alert.
    /// </summary>
    public string AlertType { get; set; } = string.Empty;

    /// <summary>
    /// The message associated with the alert.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the alert was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The severity level of the alert.
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// The user or system that created the alert.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the alert has been resolved.
    /// </summary>
    public bool IsResolved { get; set; } = false;

    /// <summary>
    /// The timestamp of the alert.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
