// Sprint83.4-FinalFixSweep2
namespace MVP_Core.Data.Models.ViewModels
{
    public class TechnicianDropdownItem
    {
        public int TechnicianId { get; set; } // Sprint83.4-FinalFixSweep2
        public string Name { get; set; } = string.Empty; // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
        public string Status { get; set; } = string.Empty; // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
        public int DispatchScore { get; set; } // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
        public bool IsOnline { get; set; } // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
        public DateTime? LastPing { get; set; } // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
        public int AssignedJobs { get; set; } // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
        public DateTime? LastUpdate { get; set; } // Sprint83.4-FinalFix: Required for Dispatcher onboarding UI binding
    }
}
// Sprint83.4-MigrationFinalGate