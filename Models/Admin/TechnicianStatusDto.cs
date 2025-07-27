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
        // Sprint83.4-IsOnlineFix: Made IsOnline assignable via object initializer for dispatcher UI binding.
        public bool IsOnline { get; init; }
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
