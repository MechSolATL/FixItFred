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
    }
}
