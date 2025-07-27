public class TechnicianLoyaltyLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public string Tier { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public int Points { get; set; }
    public string Milestone { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime AwardedAt { get; set; }
    public string Notes { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
}
