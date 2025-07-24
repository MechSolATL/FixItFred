namespace MVP_Core.Models.Admin
{
    public class TechnicianProfileDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; }
        public double CloseRate7Days { get; set; }
        public double CloseRate30Days { get; set; }
        public int CallbackCount7Days { get; set; }
        public int TotalJobsLast30Days { get; set; }
        public string[] TopZIPs { get; set; }
        public List<string> Comments { get; set; } = new();
        public DateTime LastActive { get; set; }
        public List<string> SkillTags { get; set; } = new();
    }
}
