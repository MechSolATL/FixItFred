// Sprint 47.1 Patch Log: Skill tracker and leaderboard service for admin dashboard.
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    public class SkillLeaderboardService
    {
        private readonly ApplicationDbContext _db;
        public SkillLeaderboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Sprint 47.1: Get top technicians by skill points
        public List<(Technician tech, int totalPoints)> GetTopTechniciansBySkill(string skillName, int topN = 10)
        {
            var skill = _db.TechnicianSkills.FirstOrDefault(s => s.Name == skillName);
            if (skill == null) return new List<(Technician, int)>();
            var techIds = _db.TechnicianSkillMaps.Where(m => m.SkillId == skill.Id).Select(m => m.TechnicianId).ToList();
            var leaderboard = techIds
                .Select(id => new {
                    Tech = _db.Technicians.FirstOrDefault(t => t.Id == id),
                    Points = _db.TechnicianScoreEntries.Where(e => e.TechnicianId == id && e.Type == skillName).Sum(e => e.Points)
                })
                .Where(x => x.Tech != null)
                .OrderByDescending(x => x.Points)
                .Take(topN)
                .Select(x => (x.Tech!, x.Points))
                .ToList();
            return leaderboard;
        }
    }
}
