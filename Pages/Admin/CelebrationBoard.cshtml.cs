using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class CelebrationBoardModel : PageModel
    {
        private readonly MilestoneCelebrationService _service;
        public List<EmployeeMilestoneLog> Milestones { get; set; } = new();
        public string? SurpriseMessage { get; set; }
        public CelebrationBoardModel(MilestoneCelebrationService service)
        {
            _service = service;
        }
        public async Task OnGetAsync()
        {
            Milestones = await _service._db.EmployeeMilestoneLogs.OrderByDescending(x => x.DateRecognized).ToListAsync();
        }
        public async Task OnPostAsync()
        {
            Milestones = await _service._db.EmployeeMilestoneLogs.OrderByDescending(x => x.DateRecognized).ToListAsync();
            if (Milestones.Count > 0)
            {
                var log = Milestones[0];
                var messages = _service.GenerateFunnyMessages(log.MilestoneType, log.EmployeeId);
                SurpriseMessage = messages.Count > 0 ? messages[0] : "Surprise!";
            }
            await Task.CompletedTask;
        }
    }
}
