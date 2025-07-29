// Sprint 90.1
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    // Sprint 90.1
    public class PromptTracesModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public PromptTracesModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<PromptTraceLog> Traces { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? UserId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SessionId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? VersionId { get; set; }
        public async Task OnGetAsync()
        {
            var query = _db.PromptTraceLogs.AsQueryable();
            if (!string.IsNullOrEmpty(UserId))
                query = query.Where(t => t.UserId == UserId);
            if (!string.IsNullOrEmpty(SessionId))
                query = query.Where(t => t.SessionId == SessionId);
            if (VersionId.HasValue)
                query = query.Where(t => t.PromptVersionId == VersionId.Value);
            Traces = query.OrderByDescending(t => t.CreatedAt).Take(100).ToList();
        }
    }
}
