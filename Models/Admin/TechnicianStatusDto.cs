namespace Models.Admin
{
    public class TechnicianStatusDto
    {
        public int TechnicianId { get; set; }
        public required string Name { get; set; }
        public required string Status { get; set; }
        public int AssignedJobs { get; set; }
        public DateTime LastUpdate { get; set; }
        public int DispatchScore { get; set; }
        public DateTime LastPing { get; set; }
        // Sprint 83.4-TraceFix: Resolved CS0200 — IsOnline now read/write for dispatcher UI binding
        public bool IsOnline { get; set; }
        public string DispatchTier => DispatchScore switch
        {
            >= 80 => "?? Ready",
            >= 50 => "?? At Capacity",
            _ => "?? Overloaded"
        };
        // Added for Razor compatibility
        public string TechnicianName => Name;
        public int? AssignedTechnicianId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
