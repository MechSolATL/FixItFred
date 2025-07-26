public class TechnicianRedemptionLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int PreviousTrustScore { get; set; }
    public int RestoredTrustScore { get; set; }
    public string ActionsTaken { get; set; }
    public DateTime Timestamp { get; set; }
}
