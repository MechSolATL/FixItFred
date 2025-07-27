using System;
using System.Collections.Generic;
using MVP_Core.Data.Models;
using Newtonsoft.Json;

namespace MVP_Core.Services.Admin
{
    public class IntegrityScoringService
    {
        public Dictionary<PsychologicalVector, double> ScoreVectorFromAnswers(List<string> answers)
        {
            // Dummy scoring logic: map answers to vector scores
            var scores = new Dictionary<PsychologicalVector, double>();
            foreach (PsychologicalVector vector in Enum.GetValues(typeof(PsychologicalVector)))
            {
                scores[vector] = answers.Count > 0 ? answers.Count / 15.0 : 0.0; // Placeholder
            }
            return scores;
        }

        public EmployeeOnboardingProfile GenerateInitialRiskProfile(int employeeId, List<string> answers)
        {
            var scores = ScoreVectorFromAnswers(answers);
            var profile = new EmployeeOnboardingProfile
            {
                EmployeeId = employeeId,
                SubmissionDate = DateTime.UtcNow,
                TraitScoresJson = JsonConvert.SerializeObject(scores),
                RawAnswersJson = JsonConvert.SerializeObject(answers),
                InitialRiskScore = scores.ContainsKey(PsychologicalVector.MicroManagerTendency) ? scores[PsychologicalVector.MicroManagerTendency] : 0.0,
                ReviewRequired = scores[PsychologicalVector.AuthorityResponse] > 0.8 || scores[PsychologicalVector.DeflectionPattern] > 0.8,
                EntryVector = "Onboarding"
            };
            return profile;
        }

        public bool TriggerFlagIfScoreThresholdBreached(EmployeeOnboardingProfile profile)
        {
            // Example: flag if risk score or authority/deflection is high
            var scores = JsonConvert.DeserializeObject<Dictionary<string, double>>(profile.TraitScoresJson);
            return scores.ContainsKey("MicroManagerTendency") && scores["MicroManagerTendency"] > 0.8
                || scores.ContainsKey("AuthorityResponse") && scores["AuthorityResponse"] > 0.8
                || scores.ContainsKey("DeflectionPattern") && scores["DeflectionPattern"] > 0.8;
        }

        // Sprint 76.1: Calculate ego influence score for a manager
        public double CalculateEgoInfluenceScore(int overrideCommands, int trustDelays, int disputeReversals)
        {
            // Weighted sum: override=1.5, trust delay=1.2, dispute reversal=1.3
            return overrideCommands * 1.5 + trustDelays * 1.2 + disputeReversals * 1.3;
        }

        // Sprint 76.1: Log EgoVector for manager for the week
        public void LogEgoVector(ApplicationDbContext db, int managerId, DateTime weekStart, int overrideCommands, int trustDelays, int disputeReversals, string? notes = null)
        {
            var score = CalculateEgoInfluenceScore(overrideCommands, trustDelays, disputeReversals);
            var log = new EgoVectorLog
            {
                ManagerId = managerId,
                WeekStart = weekStart,
                OverrideCommandCount = overrideCommands,
                TrustDelayCount = trustDelays,
                DisputeReversalCount = disputeReversals,
                EgoInfluenceScore = score,
                Notes = notes
            };
            db.EgoVectorLogs.Add(log);
            db.SaveChanges();
        }

        // Sprint 76.1: Trigger anonymous review if 2+ negative flags in TAM within 21 days
        public void TriggerAnonymousReviewIfNeeded(ApplicationDbContext db, int managerId, int negativeFlagCount, string contextHash)
        {
            if (negativeFlagCount >= 2)
            {
                var log = new AnonymousReviewFormLog
                {
                    ManagerId = managerId,
                    TriggeredAt = DateTime.UtcNow,
                    InitiatorCount = negativeFlagCount,
                    ContextHash = contextHash,
                    ResolutionStatus = "Pending"
                };
                db.AnonymousReviewFormLogs.Add(log);
                db.SaveChanges();
                // TODO: Notify HR-only panel (out of scope for service)
            }
        }

        // Sprint 76.1: Log weekly confidence decay for manager
        public void LogEmployeeConfidenceDecay(ApplicationDbContext db, int managerId, DateTime weekStart, double confidenceShift, double pulseScore, double interactionMatrixScore, string? notes = null)
        {
            var log = new EmployeeConfidenceDecayLog
            {
                ManagerId = managerId,
                WeekStart = weekStart,
                ConfidenceShift = confidenceShift,
                PulseScore = pulseScore,
                InteractionMatrixScore = interactionMatrixScore,
                Notes = notes
            };
            db.EmployeeConfidenceDecayLogs.Add(log);
            db.SaveChanges();
        }
    }
}
