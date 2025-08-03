using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Stats
{
    public class LeaderboardService : ILeaderboardService
    {
        public async Task<List<Data.Models.UI.TechnicianViewModel>> GetTopTechniciansAsync(int count = 6)
        {
            var list = new List<Data.Models.UI.TechnicianViewModel>
            {
                new() { Name = "Dwayne T.", Rank = 1, JobsCompleted = 47, SalesTotal = "$12,500", CustomerRating = 4.8 },
                new() { Name = "Malik R.", Rank = 2, JobsCompleted = 42, SalesTotal = "$11,200", CustomerRating = 4.7 }
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

        public async Task<string> GetMonthlyChallengeSummaryAsync()
        {
            // Mock implementation
            return await Task.FromResult("Monthly challenge summary: 85% completion rate");
        }

        public async Task<string> GetTeamEfficiencyAsync()
        {
            // Mock implementation
            return await Task.FromResult("Team efficiency: 92%");
        }
    }
}
