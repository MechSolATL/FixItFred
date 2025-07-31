namespace Models.Admin
{
    // FixItFred — Sprint 44 Final Model Recovery (AssignmentLogEntry)
    public class AssignmentLogEntry
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int TechnicianId { get; set; }
        public required string DispatcherName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public double DispatchScore { get; set; }
        public required string Tier { get; set; } = string.Empty;
        public required string Rationale { get; set; } = string.Empty;
        public List<string>? MatchedTags { get; set; }
        // Added for Razor compatibility
        public string TechnicianName { get; set; } = string.Empty;
        // Sprint 37: AI Routing Preview
        public int? AISuggestedTechnicianId { get; set; }
        public string? AISuggestedTechnicianName { get; set; }
        public double? AISuggestedScore { get; set; }
    }
}
