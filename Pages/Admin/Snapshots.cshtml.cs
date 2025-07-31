using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pages.Admin
{
    public class SnapshotsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly ReplayEngineService _replayService;

        public SnapshotsModel(ApplicationDbContext db, ReplayEngineService replayService)
        {
            _db = db;
            _replayService = replayService;
        }

        public List<SystemSnapshotLog> Snapshots { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        public string StatusMessage { get; set; } = string.Empty;

        // ✅ Final Razor SEO Metadata Model
        public SeoMetadata Seo { get; set; } = new SeoMetadata();

        public string Title => Seo.Title;
        public string MetaDescription => Seo.MetaDescription;
        public string Keywords => Seo.Keywords;
        public string Robots => Seo.Robots;

        public async Task OnGetAsync()
        {
            var query = _db.SystemSnapshotLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(s => s.Summary.Contains(SearchTerm) || s.SnapshotType.Contains(SearchTerm));
            }

            Snapshots = await Task.FromResult(query.OrderByDescending(s => s.CreatedAt).Take(100).ToList());
        }

        public async Task<IActionResult> OnPostReplayAsync(string snapshotHash)
        {
            var success = await _replayService.ReplaySnapshotAsync(snapshotHash, User?.Identity?.Name ?? "admin");

            // [Sprint91_27] Nova hard patch — Timestamp — Null coalescing guard
            StatusMessage = success ? "Replay successful." : "Replay failed.";

            await OnGetAsync();
            return Page();
        }
    }
}