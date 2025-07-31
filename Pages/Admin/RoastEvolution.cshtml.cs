using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data.Models;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class RoastEvolutionModel : PageModel
    {
        private readonly ApplicationDbContext _db; // Sprint 79.3: CS8618/CS860X warning cleanup
        private readonly RoastEvolutionEngine _engine; // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<RoastTemplate> RoastTemplates { get; set; } = new(); // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<RoastEvolutionLog> EvolutionLogs { get; set; } = new(); // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public string Prompt { get; set; } = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<RoastTemplate> AIPreview { get; set; } = new List<RoastTemplate>(); // Sprint 79.3: CS8618/CS860X warning cleanup

        public RoastEvolutionModel(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint 79.3: CS8618/CS860X warning cleanup
            _engine = new RoastEvolutionEngine(_db, new RoastFeedbackService(_db)); // Sprint 79.3: CS8618/CS860X warning cleanup
            RoastTemplates = new List<RoastTemplate>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            EvolutionLogs = new List<RoastEvolutionLog>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            Prompt = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
            AIPreview = new List<RoastTemplate>(); // Sprint 79.3: CS8618/CS860X warning cleanup
        }

        public async Task OnGetAsync()
        {
            RoastTemplates = await _db.RoastTemplates.ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
            EvolutionLogs = await _db.RoastEvolutionLogs.OrderByDescending(x => x.Timestamp).Take(100).ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
        }

        public async Task<IActionResult> OnPostPromoteAsync(int id)
        {
            var roast = await _db.RoastTemplates.FindAsync(id); // Sprint 79.3: CS8618/CS860X warning cleanup
            if (roast != null)
            {
                roast.AutoPromote = true;
                _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                {
                    RoastTemplateId = roast.Id,
                    EditType = "ManualPromote",
                    Editor = User?.Identity?.Name ?? "Admin", // Sprint 79.3: CS8618/CS860X warning cleanup
                    Timestamp = DateTime.UtcNow,
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
            var roast = await _db.RoastTemplates.FindAsync(id); // Sprint 79.3: CS8618/CS860X warning cleanup
            if (roast != null)
            {
                roast.Retired = true;
                _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                {
                    RoastTemplateId = roast.Id,
                    EditType = "ManualRetire",
                    Editor = User?.Identity?.Name ?? "Admin", // Sprint 79.3: CS8618/CS860X warning cleanup
                    Timestamp = DateTime.UtcNow,
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
                AIPreview = await _engine.GenerateAISeededRoastsAsync(Prompt); // Sprint 79.3: CS8618/CS860X warning cleanup
            }
            RoastTemplates = await _db.RoastTemplates.ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
            EvolutionLogs = await _db.RoastEvolutionLogs.OrderByDescending(x => x.Timestamp).Take(100).ToListAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
            return Page();
        }
    }
}
