public class TechnicianMoraleLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int MoraleScore { get; set; }
    public string Notes { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime Timestamp { get; set; }
    public int TrustImpact { get; set; }
}
