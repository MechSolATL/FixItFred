namespace Data.Models
{
    public class TechnicianViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int AssignedJobsCount { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
    }
}
