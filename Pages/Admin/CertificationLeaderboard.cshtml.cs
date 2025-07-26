using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class CertificationLeaderboardModel : PageModel
    {
        private readonly CertificationService _certService;
        private readonly SkillsTrackerService _skillsService;
        public List<LeaderboardEntry> Leaderboard { get; set; } = new();
        public CertificationLeaderboardModel(CertificationService certService, SkillsTrackerService skillsService)
        {
            _certService = certService;
            _skillsService = skillsService;
        }
        public void OnGet()
        {
            var techs = _skillsService.GetAllTechnicians();
            Leaderboard = techs.Select(t => new LeaderboardEntry
            {
                TechnicianName = t.FullName,
                VerifiedCerts = _certService.GetCertifications(t.Id).Count(c => c.IsVerified),
                ExpiringSoonCompletedEarly = _certService.GetCertifications(t.Id).Count(c => !c.IsExpired && c.DaysUntilExpiry <= 30 && c.IsVerified),
                TracksCompleted = _skillsService.GetProgressForTechnician(t.Id).Count(p => p.Status == "Completed"),
                IsTechOfMonth = false // Logic for Tech of the Month can be added here
            }).OrderByDescending(e => e.VerifiedCerts).ThenByDescending(e => e.TracksCompleted).ToList();
            if (Leaderboard.Count > 0)
            {
                Leaderboard[0].IsTechOfMonth = true;
            }
        }
        public class LeaderboardEntry
        {
            public string TechnicianName { get; set; } = string.Empty;
            public int VerifiedCerts { get; set; }
            public int ExpiringSoonCompletedEarly { get; set; }
            public int TracksCompleted { get; set; }
            public bool IsTechOfMonth { get; set; }
        }
    }
}
