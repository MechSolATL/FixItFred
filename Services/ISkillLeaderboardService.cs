using System.Collections.Generic;
using TechnicianModel = Data.Models.Technician;

namespace Services
{
    /// <summary>
    /// Interface for skill leaderboard service.
    /// [FixItFredComment:Sprint1004 - DI registration verified] Created interface for proper DI registration
    /// </summary>
    public interface ISkillLeaderboardService
    {
        /// <summary>
        /// Get top technicians by skill points.
        /// </summary>
        /// <param name="skillName">The skill name to query.</param>
        /// <param name="topN">Number of top results to return.</param>
        /// <returns>List of technicians with their skill points.</returns>
        List<(TechnicianModel tech, int totalPoints)> GetTopTechniciansBySkill(string skillName, int topN = 10);
    }
}