using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class TechnicianBonusService
    {
        private readonly ApplicationDbContext _db;
        private readonly TechnicianPayService _payService;
        private readonly FeedbackAnalyticsService _feedbackService;
        public TechnicianBonusService(ApplicationDbContext db, TechnicianPayService payService, FeedbackAnalyticsService feedbackService)
        {
            _db = db;
            _payService = payService;
            _feedbackService = feedbackService;
        }

        // Rule 1: 5-star streak of 3+ ? $25
        public void AwardFiveStarStreakBonus(int technicianId)
        {
            var reviews = _db.TechnicianFeedbacks.Where(f => f.TechnicianId == technicianId).OrderByDescending(f => f.FeedbackId).Take(3).ToList();
            if (reviews.Count == 3 && reviews.All(r => r.Rating == 5))
            {
                var log = new TechnicianBonusLog
                {
                    TechnicianId = technicianId,
                    BonusType = TechnicianBonusType.FiveStarStreak,
                    Amount = 25,
                    ReasonNote = "5-star streak (3 jobs)",
                    AwardedAt = DateTime.UtcNow
                };
                _db.TechnicianBonusLogs.Add(log);
                _db.SaveChanges();
            }
        }

        // Rule 2: Emergency job within SLA window ? $30
        public void AwardEmergencySLABonus(int technicianId, int jobId)
        {
            var job = _db.ServiceRequests.FirstOrDefault(j => j.Id == jobId);
            // Assume SLA window means job is closed and not delayed
            if (job != null && job.IsEmergency && job.Status == "Closed" && job.DelayMinutes <= 0)
            {
                var log = new TechnicianBonusLog
                {
                    TechnicianId = technicianId,
                    BonusType = TechnicianBonusType.EmergencySLA,
                    Amount = 30,
                    SourceJobId = jobId,
                    ReasonNote = "Emergency job resolved within SLA window",
                    AwardedAt = DateTime.UtcNow
                };
                _db.TechnicianBonusLogs.Add(log);
                _db.SaveChanges();
            }
        }

        // Rule 3: 10+ jobs/month with 0 late flags ? $50
        public void AwardPeerlessSLABonus(int technicianId, DateTime month)
        {
            var start = new DateTime(month.Year, month.Month, 1);
            var end = start.AddMonths(1);
            var jobs = _db.ServiceRequests.Where(j => j.AssignedTechnicianId == technicianId && j.CreatedAt >= start && j.CreatedAt < end).ToList();
            if (jobs.Count >= 10 && jobs.All(j => j.DelayMinutes <= 0))
            {
                var log = new TechnicianBonusLog
                {
                    TechnicianId = technicianId,
                    BonusType = TechnicianBonusType.PeerlessSLA,
                    Amount = 50,
                    ReasonNote = "10+ jobs/month, 0 late flags",
                    AwardedAt = DateTime.UtcNow
                };
                _db.TechnicianBonusLogs.Add(log);
                _db.SaveChanges();
            }
        }

        public List<TechnicianBonusLog> GetBonuses(int technicianId)
        {
            return _db.TechnicianBonusLogs.Where(b => b.TechnicianId == technicianId).OrderByDescending(b => b.AwardedAt).ToList();
        }

        public List<TechnicianBonusLog> GetBonusesByType(TechnicianBonusType type)
        {
            return _db.TechnicianBonusLogs.Where(b => b.BonusType == type).OrderByDescending(b => b.AwardedAt).ToList();
        }

        public List<TechnicianBonusLog> GetBonusesByDate(DateTime date)
        {
            return _db.TechnicianBonusLogs.Where(b => b.AwardedAt.Date == date.Date).OrderByDescending(b => b.AwardedAt).ToList();
        }
    }
}
