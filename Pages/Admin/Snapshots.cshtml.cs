using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
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
