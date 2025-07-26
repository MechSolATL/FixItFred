public class TechnicianMoraleLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int MoraleScore { get; set; }
    public string Notes { get; set; }
    public DateTime Timestamp { get; set; }
    public int TrustImpact { get; set; }
}
