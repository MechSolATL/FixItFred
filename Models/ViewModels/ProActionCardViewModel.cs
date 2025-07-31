namespace Models.ViewModels
{
    // Sprint 89.1 — Phase 3B: ProActionCardViewModel added for action rendering
    public class ProActionCardViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RiskLevel { get; set; } // e.g., High, Medium, Low
        public string ActionLabel { get; set; }
        public string ActionUrl { get; set; }
    }
}
