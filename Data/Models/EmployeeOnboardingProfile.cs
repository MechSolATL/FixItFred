using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class EmployeeOnboardingProfile
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string TraitScoresJson { get; set; } = string.Empty;
        public string RawAnswersJson { get; set; } = string.Empty;
        public double InitialRiskScore { get; set; }
        public bool ReviewRequired { get; set; }
        public string EntryVector { get; set; } = string.Empty;
    }

    public enum PsychologicalVector
    {
        Compliance,
        AuthorityResponse,
        DeflectionPattern,
        ConfidenceBias,
        SelfAwareness,
        RiskTolerance,
        PowerDistance,
        MicroManagerTendency,
        IntegritySignal,
        TrustBaseline,
        Empathy,
        Collaboration,
        Defensiveness,
        FeedbackReception,
        Adaptability
    }
}
