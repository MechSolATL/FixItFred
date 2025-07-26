public class TechnicianKarmaLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int KarmaScore { get; set; }
    public string Trend { get; set; } // Up, Down, Stable
    public string KarmaCategory { get; set; } // Composite, Manual, Audit, etc.
    public string ChangeSource { get; set; } // System, Admin, etc.
    public string Reason { get; set; }
    public DateTime Timestamp { get; set; }
}
