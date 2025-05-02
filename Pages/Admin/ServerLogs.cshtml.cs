// MVP_Core/Pages/Admin/ServerLogsModel.cs

using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MVP_Core.Pages.Admin
{
    public class ServerLogsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public List<PageVisitLog> Logs { get; set; } = new();
        public string PageViewSummary { get; set; } = "{}";
        public string BrowserSummary { get; set; } = "{}";
        public string ReferrerSummary { get; set; } = "{}";
        public List<ThreatDetectionResult> Threats { get; set; } = new();

        private const int MaxLogs = 1000; // Future: move to appsettings.json

        public ServerLogsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            Logs = await _dbContext.PageVisitLogs
                .OrderByDescending(l => l.VisitTimestamp)
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
            var threats = new List<ThreatDetectionResult>();

            var ipGroups = logs.GroupBy(l => l.IpAddress ?? "Unknown");

            foreach (var group in ipGroups)
            {
                var ip = group.Key;
                var count = group.Count();
                var errors = group.Count(l => l.ResponseStatusCode == 403 || l.ResponseStatusCode == 500);
                var suspiciousReferrer = group.Any(l => string.IsNullOrEmpty(l.Referrer) || l.Referrer.Contains("xyz") || l.Referrer.Contains("abc"));
                var botUserAgent = group.Any(l => (l.UserAgent ?? "").ToLower().Contains("bot") || (l.UserAgent ?? "").ToLower().Contains("curl") || (l.UserAgent ?? "").ToLower().Contains("python"));

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
                .GroupBy(l => l.PageUrl ?? "Unknown")
                .Select(g => new { Page = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            return new
            {
                labels = pageGroups.Select(g => g.Page).ToList(),
                values = pageGroups.Select(g => g.Count).ToList()
            };
        }

        public static object GenerateBrowserSummary(List<PageVisitLog> logs)
        {
            var browserGroups = logs
                .GroupBy(l => ParseBrowser(l.UserAgent ?? "Unknown"))
                .Select(g => new { Browser = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            return new
            {
                labels = browserGroups.Select(g => g.Browser).ToList(),
                values = browserGroups.Select(g => g.Count).ToList()
            };
        }

        public static object GenerateReferrerSummary(List<PageVisitLog> logs)
        {
            var referrerGroups = logs
                .GroupBy(l => string.IsNullOrEmpty(l.Referrer) ? "Direct/Unknown" : l.Referrer)
                .Select(g => new { Referrer = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            return new
            {
                labels = referrerGroups.Select(g => g.Referrer).ToList(),
                values = referrerGroups.Select(g => g.Count).ToList()
            };
        }

        private static string ParseBrowser(string userAgent)
        {
            if (userAgent.Contains("Chrome") && !userAgent.Contains("Edge"))
                return "Chrome";
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                return "Safari";
            if (userAgent.Contains("Firefox"))
                return "Firefox";
            if (userAgent.Contains("Edge"))
                return "Edge";
            if (userAgent.Contains("Opera") || userAgent.Contains("OPR"))
                return "Opera";
            return "Other";
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
