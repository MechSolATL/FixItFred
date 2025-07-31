namespace Models.Admin
{
    public class DispatcherFilterModel
    {
        public string? ServiceType { get; set; }
        public string? Technician { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        // Sprint 33.3 - Dispatcher Smart Filters
        public string? Zone { get; set; }
        public bool PendingOnly { get; set; }
        public bool AssignedOrDispatchedOnly { get; set; }
        public string? SearchTerm { get; set; }
        // Sprint 39 - Skill-based job filtering
        public List<string>? SkillTags { get; set; }
        // End Sprint 33.3 - Dispatcher Smart Filters
    }
}
