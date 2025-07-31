using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data;

namespace Services.Admin
{
    public class TechnicianLoyaltyService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianLoyaltyService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> GetPointsAsync(int technicianId)
        {
            return await _db.TechnicianLoyaltyLogs.Where(x => x.TechnicianId == technicianId).SumAsync(x => x.Points);
        }
        public async Task<string> GetTierAsync(int technicianId)
        {
            int points = await GetPointsAsync(technicianId);
            if (points >= 1000) return "Platinum";
            if (points >= 500) return "Gold";
            if (points >= 250) return "Silver";
            if (points >= 100) return "Bronze";
            return "None";
        }
        public async Task AwardPointsAsync(int technicianId, int points, string milestone, string notes = "")
        {
            string tier = await GetTierAsync(technicianId);
            _db.TechnicianLoyaltyLogs.Add(new TechnicianLoyaltyLog
            {
                TechnicianId = technicianId,
                Points = points,
                Tier = tier,
                Milestone = milestone,
                AwardedAt = DateTime.UtcNow,
                Notes = notes
            });
            await _db.SaveChangesAsync();
        }
        public async Task RecalculateLoyaltyAsync(int technicianId)
        {
            // Example: sum points from morale, response, service logs
            int moralePoints = await _db.TechnicianMoraleLogs.Where(x => x.TechnicianId == technicianId).SumAsync(x => x.MoraleScore);
            int responsePoints = await _db.TechnicianResponseLogs.Where(x => x.TechnicianId == technicianId).CountAsync() * 2;
            int servicePoints = await _db.ServiceRequests.Where(x => x.AssignedTechnicianId == technicianId && x.Status == "Completed").CountAsync() * 5;
            int totalPoints = moralePoints + responsePoints + servicePoints;
            string tier = "None";
            if (totalPoints >= 1000) tier = "Platinum";
            else if (totalPoints >= 500) tier = "Gold";
            else if (totalPoints >= 250) tier = "Silver";
            else if (totalPoints >= 100) tier = "Bronze";
            _db.TechnicianLoyaltyLogs.Add(new TechnicianLoyaltyLog
            {
                TechnicianId = technicianId,
                Points = totalPoints,
                Tier = tier,
                Milestone = "Recalculation",
                AwardedAt = DateTime.UtcNow,
                Notes = "Auto loyalty recalculation"
            });
            await _db.SaveChangesAsync();
        }
        public async Task<List<TechnicianLoyaltyLog>> GetLoyaltyHistoryAsync(int technicianId)
        {
            return await _db.TechnicianLoyaltyLogs.Where(x => x.TechnicianId == technicianId).OrderByDescending(x => x.AwardedAt).ToListAsync();
        }
        public async Task<List<TechnicianLoyaltyLog>> GetByTierAsync(string tier)
        {
            return await _db.TechnicianLoyaltyLogs.Where(x => x.Tier == tier).OrderByDescending(x => x.AwardedAt).ToListAsync();
        }
        public async Task<List<TechnicianLoyaltyLog>> GetMostImprovedAsync()
        {
            // Example: sort by points delta in last 30 days
            var logs = await _db.TechnicianLoyaltyLogs.Where(x => x.AwardedAt > DateTime.UtcNow.AddDays(-30)).ToListAsync();
            return logs.OrderByDescending(x => x.Points).ToList();
        }
    }
}
