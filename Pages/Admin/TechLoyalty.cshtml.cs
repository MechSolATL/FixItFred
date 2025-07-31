using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Pages.Admin
{
    public class TechLoyaltyModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TechLoyaltyModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Data.Models.Technician> Technicians { get; set; } = new();
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
