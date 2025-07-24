namespace MVP_Core.Models.Admin
{
    public class AssignmentLogEntry
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int TechnicianId { get; set; }
        public string DispatcherName { get; set; }
        public DateTime Timestamp { get; set; }
        public int DispatchScore { get; set; }
        public string Tier { get; set; }
        public string Rationale { get; set; }
        public List<string> MatchedTags { get; set; } = new();
    }
}
