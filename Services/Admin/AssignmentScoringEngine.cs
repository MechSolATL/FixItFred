using System;
using System.Collections.Generic;
using System.Linq;
using MVP_Core.Data.Models;
using MVP_Core.Models.Admin;

namespace MVP_Core.Services.Admin
{
    /// <summary>
    /// AssignmentScoringEngine: Calculates assignment scores and confidence for technicians.
    /// Factors: ETA, reviews, streaks, SLA wins, availability, burnout, bonus cooldown, escalation.
    /// </summary>
    public class AssignmentScoringEngine
    {
        public class TechnicianAssignmentScore
        {
            public int TechnicianId { get; set; }
            public double Score { get; set; }
            public double Confidence { get; set; }
            public Dictionary<string, double> Factors { get; set; } = new();
            public bool IsEscalationRisk { get; set; }
        }

        // Confidence scoring schema:
        // Confidence = 1.0 - (Sum of penalty factors / Max possible penalty)
        // Factors: ETA, reviews, streaks, SLA wins, availability, burnout, bonus cooldown, escalation
        public List<TechnicianAssignmentScore> GetScores(ServiceRequest request, List<TechnicianStatusDto> technicians, ApplicationDbContext db)
        {
            var scores = new List<TechnicianAssignmentScore>();
            var now = DateTime.UtcNow;
            foreach (var tech in technicians)
            {
                var score = new TechnicianAssignmentScore
                {
                    TechnicianId = tech.TechnicianId,
                    Factors = new Dictionary<string, double>()
                };
                // ETA factor (lower ETA = higher score)
                double etaMinutes = 30;
                try
                {
                    etaMinutes = db.ScheduleQueues.Where(q => q.TechnicianId == tech.TechnicianId && q.ServiceRequestId == request.Id)
                        .Select(q => q.OptimizedETA != null ? q.OptimizedETA.Value.TotalMinutes : 30)
                        .DefaultIfEmpty(30).FirstOrDefault();
                }
                catch { }
                score.Factors["ETA"] = etaMinutes;
                // Reviews factor (higher avg = higher score)
                double avgReview = db.CustomerReviews.Where(r => r.ServiceRequestId == request.Id)
                    .Select(r => (double)r.Rating)
                    .DefaultIfEmpty(4.0).Average();
                score.Factors["Reviews"] = avgReview;
                // Streaks factor (recent punch-ins as proxy for SLA wins)
                int slaWins = db.TechnicianAuditLogs.Count(l => l.TechnicianId == tech.TechnicianId && l.ActionType == TechnicianAuditActionType.PunchIn && l.Timestamp > now.AddDays(-30));
                score.Factors["SLAWins"] = slaWins;
                // Availability factor
                score.Factors["Availability"] = tech.Status == "Available" ? 1 : 0;
                // Burnout factor (active jobs > 4 = penalty)
                int activeJobs = db.ScheduleQueues.Count(q => q.TechnicianId == tech.TechnicianId && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
                score.Factors["Burnout"] = activeJobs > 4 ? 1 : 0;
                // Bonus cooldown (recent bonus = penalty)
                bool bonusCooldown = db.TechnicianBonusLogs.Any(b => b.TechnicianId == tech.TechnicianId && b.AwardedAt > now.AddDays(-7));
                score.Factors["BonusCooldown"] = bonusCooldown ? 1 : 0;
                // Escalation risk (recent escalations for this request)
                int escalations = db.KanbanHistoryLogs.Where(l => l.ServiceRequestId == request.Id && l.ToStatus == "Escalated" && l.Timestamp > now.AddDays(-14)).Count();
                score.Factors["Escalation"] = escalations;
                score.IsEscalationRisk = escalations > 0;
                // Predictive assignment scoring
                // Base: 100 - (ETA) + (Reviews*5) + (SLAWins*2) + (Availability*10) - (Burnout*15) - (BonusCooldown*10) - (Escalation*20)
                score.Score = 100 - etaMinutes + (avgReview * 5) + (slaWins * 2) + (score.Factors["Availability"] * 10)
                    - (score.Factors["Burnout"] * 15) - (score.Factors["BonusCooldown"] * 10) - (escalations * 20);
                // Confidence: 1.0 - (burnout+bonusCooldown+escalation)/maxPenalty
                double penalty = (score.Factors["Burnout"] * 15) + (score.Factors["BonusCooldown"] * 10) + (escalations * 20);
                score.Confidence = Math.Max(0.1, 1.0 - penalty / 100.0);
                scores.Add(score);
            }
            // Sort by score descending
            return scores.OrderByDescending(s => s.Score).ToList();
        }
    }
}
