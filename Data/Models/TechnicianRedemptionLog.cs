public class TechnicianRedemptionLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int PreviousTrustScore { get; set; }
    public int RestoredTrustScore { get; set; }
    public string ActionsTaken { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime Timestamp { get; set; }
}
