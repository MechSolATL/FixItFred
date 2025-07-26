public class TechnicianPatternProfile
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public string PatternType { get; set; } // "Escalation Spike", "Warning Clusters", etc.
    public string Description { get; set; }
    public DateTime DetectedAt { get; set; }
    public string RiskLevel { get; set; } // Low, Medium, High
    public bool IsAutoFlagged { get; set; }
}
