using Data;
using Data.Models;

namespace Pages.Admin
{
    [ValidateAntiForgeryToken] // Applied globally for Razor Pages
    public class BackupLogModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public BackupLogModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<BackupLog> BackupLogs { get; set; } = [];
        [BindProperty(SupportsGet = true)] public string StatusFilter { get; set; } = "All";
        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; } = "Date_desc";
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            IQueryable<BackupLog> query = _db.BackupLogs;

            if (StatusFilter != "All")
            {
                query = query.Where(x => x.Status == StatusFilter);
            }

            // Sorting
            query = SortOrder switch
            {
                "Date" => query.OrderBy(x => x.Date),
                "Status" => query.OrderBy(x => x.Status),
                "Status_desc" => query.OrderByDescending(x => x.Status),
                _ => query.OrderByDescending(x => x.Date) // Default
            };

            int pageSize = 25;
            int totalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            BackupLogs = await query.Skip((PageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["Title"] = "Backup Logs";
            return Page();
        }

        public string GetSortOrder(string column)
        {
            return SortOrder == column ? $"{column}_desc" : column;
        }

        public string GetSortIcon(string column)
        {
            return SortOrder == column ? "??" :
                   SortOrder == $"{column}_desc" ? "??" : "";
        }

        public string GetBadgeColor(string status)
        {
            return status switch
            {
                "Success" => "bg-green-200 text-green-800",
                "Failed" => "bg-red-200 text-red-800",
                "Warning" => "bg-yellow-200 text-yellow-800",
                _ => "bg-gray-200 text-gray-800"
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            BackupLog? log = await _db.BackupLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _ = _db.BackupLogs.Remove(log);
            _ = _db.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostInsertTestLog()
        {
            BackupLog log = new()
            {
                Date = DateTime.Now,
                Status = "Success",
                BackupType = "Full",
                Message = "Test backup completed.",
                BackupSizeMB = 12,
                DurationSeconds = 4,
                SourceServer = Environment.MachineName
            };

            _ = _db.BackupLogs.Add(log);
            _ = _db.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostClearOldLogs()
        {
            DateTime cutoff = DateTime.Now.AddDays(-30);
            IQueryable<BackupLog> oldLogs = _db.BackupLogs.Where(x => x.Date < cutoff);
            _db.BackupLogs.RemoveRange(oldLogs);
            _ = _db.SaveChanges();
            return RedirectToPage();
        }
    }
}
