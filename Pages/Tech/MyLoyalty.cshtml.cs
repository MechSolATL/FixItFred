using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MVP_Core.Pages.Tech
{
    public class MyLoyaltyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public MyLoyaltyModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<TechMilestone> Milestones { get; set; } = new();
        public List<MilestoneAuditLog> UnlockedMilestones { get; set; } = new();
        public async Task<IActionResult> OnGetAsync()
        {
            // Get technician ID from claims or query
            int techId = 0;
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(idClaim, out techId);
            }
            if (techId == 0 && Request.Query.ContainsKey("techId"))
                int.TryParse(Request.Query["techId"], out techId);
            if (techId == 0)
                return Unauthorized();
            Milestones = await _context.TechMilestones.ToListAsync();
            UnlockedMilestones = await _context.MilestoneAuditLogs.Where(x => x.TechnicianId == techId && x.Action == "Unlocked").ToListAsync();
            return Page();
        }
    }
}
