using System;
using System.Collections.Generic;
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
        // Sprint 83.4-TraceFix: Resolved CS1061 — Added missing properties for READMEGenerator
        public string UserId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public List<string> FilePaths { get; set; } = new();
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
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
