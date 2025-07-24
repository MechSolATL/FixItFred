namespace MVP_Core.Models.Admin
{
    public class TechnicianStatusDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int AssignedJobs { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
