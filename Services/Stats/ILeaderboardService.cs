using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Stats
{
    public interface ILeaderboardService
    {
        Task<List<Data.Models.UI.TechnicianViewModel>> GetTopTechniciansAsync(int count = 6);
        Task<string> GetEfficiencyRateAsync();
        Task<string> GetMonthlyChallengeAsync();
        Task<string> GetMonthlyChallengeSummaryAsync();
        Task<string> GetTeamEfficiencyAsync();
    }
}
