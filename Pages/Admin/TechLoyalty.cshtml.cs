using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Admin
{
    public class TechLoyaltyModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TechLoyaltyModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Technician> Technicians { get; set; } = new();
        public List<TechMilestone> Milestones { get; set; } = new();
        public List<MilestoneAuditLog> UnlockedMilestones { get; set; } = new();

        public async Task OnGetAsync()
        {
            Technicians = await _context.Technicians.ToListAsync();
            Milestones = await _context.TechMilestones.ToListAsync();
            UnlockedMilestones = await _context.MilestoneAuditLogs.ToListAsync();
        }
    }
}
