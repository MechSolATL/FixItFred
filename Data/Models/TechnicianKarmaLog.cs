public class TechnicianKarmaLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int KarmaScore { get; set; }
    public string Trend { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string KarmaCategory { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string ChangeSource { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string Reason { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime Timestamp { get; set; }
}
