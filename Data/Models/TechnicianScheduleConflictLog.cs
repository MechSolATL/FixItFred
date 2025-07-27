public class TechnicianScheduleConflictLog
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }
    public int JobId { get; set; }
    public DateTime ConflictDetectedAt { get; set; }
    public string ConflictType { get; set; } // Overlap, TravelWindow, PriorityClash
    public string Details { get; set; }
    public bool IsAcknowledged { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionSuggestion { get; set; }
    public string? ManualOverrideNote { get; set; }
}
