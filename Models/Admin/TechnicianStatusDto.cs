namespace MVP_Core.Models.Admin
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
        public bool IsOnline => (DateTime.UtcNow - LastPing).TotalMinutes < 10;
        public string DispatchTier => DispatchScore switch
        {
            >= 80 => "?? Ready",
            >= 50 => "?? At Capacity",
            _     => "?? Overloaded"
        };
        // Added for Razor compatibility
        public string TechnicianName => Name;
        public int? AssignedTechnicianId { get; set; }
    }
}
