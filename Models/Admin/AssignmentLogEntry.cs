namespace MVP_Core.Models.Admin
{
    public class AssignmentLogEntry
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int TechnicianId { get; set; }
        public required string DispatcherName { get; set; }
        public DateTime Timestamp { get; set; }
        public double DispatchScore { get; set; }
        public required string Tier { get; set; }
        public required string Rationale { get; set; }
        public List<string>? MatchedTags { get; set; }
        // Added for Razor compatibility
        public string TechnicianName { get; set; } = string.Empty;
    }
}
