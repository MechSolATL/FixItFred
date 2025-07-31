// Sprint 76.1 Patch: Model for logging ego vector events and scores for managers/employees
namespace Data.Models
{
    public class EgoVectorLog
    {
        public int Id { get; set; }
        public int ManagerId { get; set; }
        public DateTime WeekStart { get; set; }
        public int OverrideCommandCount { get; set; }
        public int TrustDelayCount { get; set; }
        public int DisputeReversalCount { get; set; }
        public double EgoInfluenceScore { get; set; }
        public string? Notes { get; set; }
    }

    // Sprint 76.1 Patch: Model for triggering and logging anonymous review forms for managers
    public class AnonymousReviewFormLog
    {
        public int Id { get; set; }
        public int ManagerId { get; set; }
        public DateTime TriggeredAt { get; set; }
        public int InitiatorCount { get; set; }
        public string ContextHash { get; set; } = string.Empty;
        public string ResolutionStatus { get; set; } = string.Empty;
    }

    // Sprint 76.1 Patch: Model for logging weekly employee confidence decay for HR review
    public class EmployeeConfidenceDecayLog
    {
        public int Id { get; set; }
        public int ManagerId { get; set; }
        public DateTime WeekStart { get; set; }
        public double ConfidenceShift { get; set; }
        public double PulseScore { get; set; }
        public double InteractionMatrixScore { get; set; }
        public string? Notes { get; set; }
    }
}
