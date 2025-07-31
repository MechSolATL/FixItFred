// Sprint 91.8 - Part 5.B
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;

namespace Pages.Admin
{
    // Sprint 90.1 — Admin view for recent prompt trace logs
    public class PromptTracesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PromptTracesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PromptTraceLog> Traces { get; set; } = new();

        public async Task OnGetAsync()
        {
            Traces = await _context.PromptTraceLogs
                .OrderByDescending(p => p.ExecutedAt)
                .Take(100)
                .ToListAsync();
        }
    }
}
