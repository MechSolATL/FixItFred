// MVP_Core/Pages/Admin/ServerLogsModel.cs

using Data;
using Data.Models;
using System.Text.Json;

namespace Pages.Admin
{
    public class ServerLogsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public List<PageVisitLog> Logs { get; set; } = [];
        public string PageViewSummary { get; set; } = "{}";
        public string BrowserSummary { get; set; } = "{}";
        public string ReferrerSummary { get; set; } = "{}";
        public List<ThreatDetectionResult> Threats { get; set; } = [];

        private const int MaxLogs = 1000; // Future: move to appsettings.json

        public ServerLogsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            Logs = await _dbContext.PageVisitLogs
                .OrderByDescending(static l => l.VisitTimestamp)
                .Take(MaxLogs)
                .AsNoTracking()
                .ToListAsync();

            Threats = ThreatAnalyzer.AnalyzeThreats(Logs);

            PageViewSummary = JsonSerializer.Serialize(ThreatAnalyzer.GeneratePageViewSummary(Logs));
            BrowserSummary = JsonSerializer.Serialize(ThreatAnalyzer.GenerateBrowserSummary(Logs));
            ReferrerSummary = JsonSerializer.Serialize(ThreatAnalyzer.GenerateReferrerSummary(Logs));
        }
    }

    // ThreatAnalyzer moved to proper helper class
    public static class ThreatAnalyzer
    {
        public static List<ThreatDetectionResult> AnalyzeThreats(List<PageVisitLog> logs)
        {
            List<ThreatDetectionResult> threats = [];

            IEnumerable<IGrouping<string, PageVisitLog>> ipGroups = logs.GroupBy(static l => l.IpAddress ?? "Unknown");

            foreach (IGrouping<string, PageVisitLog> group in ipGroups)
            {
                string ip = group.Key;
                int count = group.Count();
                int errors = group.Count(static l => l.ResponseStatusCode == 403 || l.ResponseStatusCode == 500);
                bool suspiciousReferrer = group.Any(static l => string.IsNullOrEmpty(l.Referrer) || l.Referrer.Contains("xyz") || l.Referrer.Contains("abc"));
                bool botUserAgent = group.Any(static l => (l.UserAgent ?? "").ToLower().Contains("bot") || (l.UserAgent ?? "").ToLower().Contains("curl") || (l.UserAgent ?? "").ToLower().Contains("python"));

                if (count >= 20 || errors >= 10 || suspiciousReferrer || botUserAgent)
                {
                    threats.Add(new ThreatDetectionResult
                    {
                        IpAddress = ip,
                        TotalHits = count,
                        ErrorCount = errors,
                        IsSuspiciousReferrer = suspiciousReferrer,
                        IsBotDetected = botUserAgent
                    });
                }
            }

            return threats;
        }

        public static object GeneratePageViewSummary(List<PageVisitLog> logs)
        {
            var pageGroups = logs
                .GroupBy(static l => l.PageUrl ?? "Unknown")
                .Select(static g => new { Page = g.Key, Count = g.Count() })
                .OrderByDescending(static g => g.Count)
                .ToList();

            return new
            {
                labels = pageGroups.Select(static g => g.Page).ToList(),
                values = pageGroups.Select(static g => g.Count).ToList()
            };
        }

        public static object GenerateBrowserSummary(List<PageVisitLog> logs)
        {
            var browserGroups = logs
                .GroupBy(static l => ParseBrowser(l.UserAgent ?? "Unknown"))
                .Select(static g => new { Browser = g.Key, Count = g.Count() })
                .OrderByDescending(static g => g.Count)
                .ToList();

            return new
            {
                labels = browserGroups.Select(static g => g.Browser).ToList(),
                values = browserGroups.Select(static g => g.Count).ToList()
            };
        }

        public static object GenerateReferrerSummary(List<PageVisitLog> logs)
        {
            var referrerGroups = logs
                .GroupBy(static l => string.IsNullOrEmpty(l.Referrer) ? "Direct/Unknown" : l.Referrer)
                .Select(static g => new { Referrer = g.Key, Count = g.Count() })
                .OrderByDescending(static g => g.Count)
                .ToList();

            return new
            {
                labels = referrerGroups.Select(static g => g.Referrer).ToList(),
                values = referrerGroups.Select(static g => g.Count).ToList()
            };
        }

        private static string ParseBrowser(string userAgent)
        {
            return userAgent.Contains("Chrome") && !userAgent.Contains("Edge")
                ? "Chrome"
                : userAgent.Contains("Safari") && !userAgent.Contains("Chrome")
                ? "Safari"
                : userAgent.Contains("Firefox")
                ? "Firefox"
                : userAgent.Contains("Edge") ? "Edge" : userAgent.Contains("Opera") || userAgent.Contains("OPR") ? "Opera" : "Other";
        }
    }

    public class ThreatDetectionResult
    {
        public string IpAddress { get; set; } = "";
        public int TotalHits { get; set; }
        public int ErrorCount { get; set; }
        public bool IsSuspiciousReferrer { get; set; }
        public bool IsBotDetected { get; set; }
    }
}
