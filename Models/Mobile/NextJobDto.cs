namespace MVP_Core.Models.Mobile
{
    public class NextJobDto
    {
        public int TechnicianId { get; set; }
        [Required]
        public string TechnicianName { get; set; } = string.Empty;
        [Required]
        public string JobSummary { get; set; } = string.Empty;
        [Required]
        public DateTime ETA { get; set; }
        [Required]
        public string Address { get; set; } = string.Empty;
        public string DispatcherNote { get; set; } = string.Empty;
        public DateTime LastPing { get; set; }
    }
}
