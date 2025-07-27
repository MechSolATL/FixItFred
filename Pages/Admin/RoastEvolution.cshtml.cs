using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class RoastEvolutionModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly RoastEvolutionEngine _engine;
        public List<RoastTemplate> RoastTemplates { get; set; } = new();
        public List<RoastEvolutionLog> EvolutionLogs { get; set; } = new();
        [BindProperty]
        public string Prompt { get; set; }
        public List<RoastTemplate> AIPreview { get; set; }

        public RoastEvolutionModel(ApplicationDbContext db)
        {
            _db = db;
            _engine = new RoastEvolutionEngine(db, new RoastFeedbackService(db));
        }

        public async Task OnGetAsync()
        {
            RoastTemplates = await _db.RoastTemplates.ToListAsync();
            EvolutionLogs = await _db.RoastEvolutionLogs.OrderByDescending(x => x.Timestamp).Take(100).ToListAsync();
        }

        public async Task<IActionResult> OnPostPromoteAsync(int id)
        {
            var roast = await _db.RoastTemplates.FindAsync(id);
            if (roast != null)
            {
                roast.AutoPromote = true;
                _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                {
                    RoastTemplateId = roast.Id,
                    EditType = "ManualPromote",
                    Editor = User.Identity?.Name ?? "Admin",
                    Timestamp = System.DateTime.UtcNow,
                    Notes = "Manual promote.",
                    Promoted = true,
                    PreviousMessage = roast.Message,
                    NewMessage = roast.Message
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRetireAsync(int id)
        {
            var roast = await _db.RoastTemplates.FindAsync(id);
            if (roast != null)
            {
                roast.Retired = true;
                _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                {
                    RoastTemplateId = roast.Id,
                    EditType = "ManualRetire",
                    Editor = User.Identity?.Name ?? "Admin",
                    Timestamp = System.DateTime.UtcNow,
                    Notes = "Manual retire.",
                    Retired = true,
                    PreviousMessage = roast.Message,
                    NewMessage = roast.Message
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAISeedAsync()
        {
            if (!string.IsNullOrWhiteSpace(Prompt))
            {
                AIPreview = await _engine.GenerateAISeededRoastsAsync(Prompt);
            }
            RoastTemplates = await _db.RoastTemplates.ToListAsync();
            EvolutionLogs = await _db.RoastEvolutionLogs.OrderByDescending(x => x.Timestamp).Take(100).ToListAsync();
            return Page();
        }
    }
}
