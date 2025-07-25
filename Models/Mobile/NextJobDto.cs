namespace MVP_Core.Models.Mobile
{
    public class NextJobDto
    {
        public int TechnicianId { get; set; }
        public required string TechnicianName { get; set; }
        public required string JobSummary { get; set; }
        public DateTime ETA { get; set; }
        public required string Address { get; set; }
        public required string DispatcherNote { get; set; }
        public DateTime LastPing { get; set; }
    }
}
