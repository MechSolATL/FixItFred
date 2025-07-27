public class TechnicianLoyaltyLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public string Tier { get; set; } // Bronze, Silver, Gold, Platinum
    public int Points { get; set; }
    public string Milestone { get; set; }
    public DateTime AwardedAt { get; set; }
    public string Notes { get; set; }
}
