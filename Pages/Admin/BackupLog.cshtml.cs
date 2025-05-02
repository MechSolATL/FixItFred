using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Models;
using MVP_Core.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MVP_Core.Pages.Admin
{
    public class BackupLogModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BackupLogModel> _logger;

        public BackupLogModel(ApplicationDbContext context, ILogger<BackupLogModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public PaginatedList<BackupLog> BackupLogs { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "DateDesc";

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "All";

        private const int PageSize = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.BackupLogs.AsQueryable();

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                query = query.Where(x => x.Status == StatusFilter);
            }

            query = SortOrder switch
            {
                "Date" => query.OrderBy(x => x.Date),
                "DateDesc" => query.OrderByDescending(x => x.Date),
                "Status" => query.OrderBy(x => x.Status),
                "StatusDesc" => query.OrderByDescending(x => x.Status),
                _ => query.OrderByDescending(x => x.Date),
            };

            BackupLogs = await PaginatedList<BackupLog>.CreateAsync(query, PageIndex, PageSize);
            return Page();
        }

        public string GetSortOrder(string column)
        {
            return column switch
            {
                "Date" => SortOrder == "Date" ? "DateDesc" : "Date",
                "Status" => SortOrder == "Status" ? "StatusDesc" : "Status",
                _ => "DateDesc",
            };
        }

        public string GetSortIcon(string column)
        {
            return (column, SortOrder) switch
            {
                ("Date", "Date") => "↑",
                ("Date", "DateDesc") => "↓",
                ("Status", "Status") => "↑",
                ("Status", "StatusDesc") => "↓",
                _ => string.Empty,
            };
        }

        public string GetBadgeColor(string status)
        {
            return status switch
            {
                "Success" => "bg-green-100 text-green-800",
                "Failed" => "bg-red-100 text-red-800",
                "Warning" => "bg-yellow-100 text-yellow-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostInsertTestLogAsync()
        {
            try
            {
                var statuses = new[] { "Success", "Failed", "Warning" };
                var random = new Random();
                var randomStatus = statuses[random.Next(statuses.Length)];

                var newLog = new BackupLog
                {
                    Date = DateTime.UtcNow,
                    Status = randomStatus,
                    Message = $"Test backup event generated ({randomStatus}) at {DateTime.UtcNow:HH:mm:ss}"
                };

                _context.BackupLogs.Add(newLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Test backup log inserted: {Status} at {Time}", randomStatus, DateTime.UtcNow);

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting test backup log.");
                ModelState.AddModelError(string.Empty, "Failed to insert test backup log.");
                return await OnGetAsync();
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostClearOldLogsAsync()
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-30);

                var oldLogs = await _context.BackupLogs
                    .Where(log => log.Date < cutoffDate)
                    .ToListAsync();

                if (oldLogs.Count > 0)
                {
                    _context.BackupLogs.RemoveRange(oldLogs);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cleared {Count} old backup logs older than 30 days.", oldLogs.Count);
                }
                else
                {
                    _logger.LogInformation("No backup logs older than 30 days found to delete.");
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing old backup logs.");
                ModelState.AddModelError(string.Empty, "Failed to clear old backup logs.");
                return await OnGetAsync();
            }
        }
    }
}
