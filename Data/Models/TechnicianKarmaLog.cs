public class TechnicianKarmaLog
{
    /// <summary>
    /// The unique identifier for the technician karma log entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the technician associated with the karma log entry.
    /// </summary>
    public int TechnicianId { get; set; }

    /// <summary>
    /// The karma score of the technician.
    /// </summary>
    public int KarmaScore { get; set; }

    /// <summary>
    /// The trend of the karma score (e.g., increasing, decreasing).
    /// </summary>
    public string Trend { get; set; } = string.Empty;

    /// <summary>
    /// The category of the karma score (e.g., performance, behavior).
    /// </summary>
    public string KarmaCategory { get; set; } = string.Empty;

    /// <summary>
    /// The source of the change in karma score.
    /// </summary>
    public string ChangeSource { get; set; } = string.Empty;

    /// <summary>
    /// The reason for the change in karma score.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the karma log entry was created.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
