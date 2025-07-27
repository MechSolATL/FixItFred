public class TechnicianPatternProfile
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public string PatternType { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string Description { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime DetectedAt { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public bool IsAutoFlagged { get; set; }
}
