// Sprint 44 – Message Export + Digest
using MVP_Core.Data;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    public class NotificationDigestService
    {
        private readonly ApplicationDbContext _db;
        public NotificationDigestService(ApplicationDbContext db)
        {
            _db = db;
        }
        public class DigestSummary
        {
            public List<UnreadThreadInfo> UnreadThreads { get; set; } = new();
            public List<OverdueTicketInfo> OverdueTickets { get; set; } = new();
            public List<SatisfactionScoreInfo> RecentSatisfactionScores { get; set; } = new();
            public string HtmlBody { get; set; } = string.Empty;
        }
        public class UnreadThreadInfo { public int RequestId; public string Subject = string.Empty; public int UnreadCount; }
        public class OverdueTicketInfo { public int RequestId; public string Subject = string.Empty; public int OverdueMinutes; }
        public class SatisfactionScoreInfo { public int RequestId; public int Score; public string Submitter = string.Empty; }

        public async Task<DigestSummary> GenerateDigestPreviewAsync()
        {
            // Simulate data for preview
            var digest = new DigestSummary
            {
                UnreadThreads = new List<UnreadThreadInfo> {
                    new() { RequestId = 101, Subject = "Leaky Faucet", UnreadCount = 2 },
                    new() { RequestId = 102, Subject = "No Heat", UnreadCount = 1 }
                },
                OverdueTickets = new List<OverdueTicketInfo> {
                    new() { RequestId = 201, Subject = "Clogged Drain", OverdueMinutes = 45 }
                },
                RecentSatisfactionScores = new List<SatisfactionScoreInfo> {
                    new() { RequestId = 301, Score = 5, Submitter = "Alice" },
                    new() { RequestId = 302, Score = 3, Submitter = "Bob" }
                }
            };
            digest.HtmlBody = GenerateDigestHtml(digest);
            return await Task.FromResult(digest);
        }

        public string GenerateDigestHtml(DigestSummary digest)
        {
            // Simple HTML formatting for email
            var html = "<h2>Daily Notification Digest</h2>";
            html += "<h4>Unread Threads</h4><ul>" + string.Join("", digest.UnreadThreads.Select(t => $"<li>Request #{t.RequestId}: {t.Subject} ({t.UnreadCount} unread)</li>")) + "</ul>";
            html += "<h4>Overdue Tickets</h4><ul>" + string.Join("", digest.OverdueTickets.Select(t => $"<li>Request #{t.RequestId}: {t.Subject} (Overdue by {t.OverdueMinutes} min)</li>")) + "</ul>";
            html += "<h4>Recent Satisfaction Scores</h4><ul>" + string.Join("", digest.RecentSatisfactionScores.Select(s => $"<li>Request #{s.RequestId}: {s.Score}/5 ({s.Submitter})</li>")) + "</ul>";
            return html;
        }

        public async Task<string> TriggerDigestAsync(bool sendEmail, INotificationService notificationService, string toEmail)
        {
            var digest = await GenerateDigestPreviewAsync();
            if (sendEmail)
            {
                await notificationService.SendEmailAsync(toEmail, "Daily Notification Digest", digest.HtmlBody);
                return "Digest email sent.";
            }
            return digest.HtmlBody;
        }
    }
}
