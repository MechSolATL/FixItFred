namespace MVP_Core.Models.Mobile
{
    public class NextJobDto
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public string JobSummary { get; set; }
        public DateTime ETA { get; set; }
        public string Address { get; set; }
        public string DispatcherNote { get; set; }
        public DateTime LastPing { get; set; }
    }
}
