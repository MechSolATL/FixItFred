namespace MVP_Core.Models.Admin
{
    public class TechnicianStatusDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int AssignedJobs { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastPing { get; set; }
        public bool IsOnline => (DateTime.UtcNow - LastPing).TotalMinutes < 10;
        public int DispatchScore { get; set; }
        public string DispatchTier => DispatchScore switch
        {
            >= 80 => "?? Ready",
            >= 50 => "?? At Capacity",
            _     => "?? Overloaded"
        };
    }
}
