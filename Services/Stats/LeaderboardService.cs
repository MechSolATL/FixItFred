using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Stats
{
    public class LeaderboardService : ILeaderboardService
    {
        public async Task<List<TechnicianViewModel>> GetTopTechniciansAsync(int count = 6)
        {
            var list = new List<TechnicianViewModel>
            {
                new() { TechnicianName = "Dwayne T.", CurrentTier = "Gold", TotalPoints = 47, LastRewardSentAt = DateTime.UtcNow, IsEligibleForReward = true },
                new() { TechnicianName = "Malik R.", CurrentTier = "Silver", TotalPoints = 42, LastRewardSentAt = DateTime.UtcNow, IsEligibleForReward = true }
            };
            return await Task.FromResult(list);
        }

        public async Task<string> GetEfficiencyRateAsync()
        {
            // Mock implementation
            return await Task.FromResult("85%");
        }

        public async Task<string> GetMonthlyChallengeAsync()
        {
            // Mock implementation
            return await Task.FromResult("Complete 100 jobs");
        }
    }
}
