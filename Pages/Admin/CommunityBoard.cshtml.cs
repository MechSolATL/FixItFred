using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Models;
using Data.Models;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CommunityBoardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        public PermissionService PermissionService { get; }
        public AdminUser AdminUser { get; }
        private static readonly string QuoteCacheKey = "DailyQuote";
        private static readonly string AudioCacheKey = "DailyAudio";
        private static readonly string SpotlightCacheKey = "TechSpotlight";
        public List<QuoteMessage> Announcements { get; set; } = new();
        public List<Whisper> Whispers { get; set; } = new();
        public QuoteMessage? DailyQuote { get; set; }
        public string? AudioFile { get; set; }
        public string? TechSpotlight { get; set; }
        public List<PulseSummaryDto> PulseSummary { get; set; } = new();

        public CommunityBoardModel(ApplicationDbContext db, IMemoryCache cache, PermissionService permissionService)
        {
            _db = db;
            _cache = cache;
            PermissionService = permissionService;
            AdminUser = HttpContext?.Items["AdminUser"] as AdminUser ?? new AdminUser { EnabledModules = new List<string>() };
        }

        public void OnGet()
        {
            Announcements = _db.QuoteMessages.OrderByDescending(q => q.CreatedAt).Take(5).ToList();
            Whispers = _db.Whispers.Where(w => w.SentAt.Date == DateTime.UtcNow.Date).OrderByDescending(w => w.SentAt).Take(10).ToList();
            DailyQuote = _cache.GetOrCreate(QuoteCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _db.QuoteMessages.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            });
            AudioFile = _cache.GetOrCreate(AudioCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var audioDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audio", "daily");
                if (Directory.Exists(audioDir))
                {
                    var files = Directory.GetFiles(audioDir);
                    if (files.Length > 0)
                        return Path.GetFileName(files[new Random().Next(files.Length)]);
                }
                return null;
            });
            TechSpotlight = _cache.GetOrCreate(SpotlightCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var certified = _db.LeaderboardEntries.Where(e => e.BadgesJson.Contains("Certified")).ToList();
                if (certified.Count > 0)
                    return certified[new Random().Next(certified.Count)].Name;
                return "No certified tech today.";
            });
            PulseSummary = _db.TechPulseLogs
                .Where(p => p.Date == DateTime.UtcNow.Date)
                .GroupBy(p => p.Emoji)
                .Select(g => new PulseSummaryDto { Emoji = g.Key, Count = g.Count() })
                .ToList();
        }

        public IActionResult OnPostAnnouncement(string Announcement)
        {
            if (!string.IsNullOrWhiteSpace(Announcement))
            {
                _db.QuoteMessages.Add(new QuoteMessage { Message = Announcement, Author = User.Identity?.Name ?? "Manager" });
                _db.SaveChanges();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostSendWhisper(string Recipient, string Message)
        {
            if (!string.IsNullOrWhiteSpace(Recipient) && !string.IsNullOrWhiteSpace(Message))
            {
                _db.Whispers.Add(new Whisper { Sender = User.Identity?.Name ?? "Admin", Recipient = Recipient, Message = Message });
                _db.SaveChanges();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostReact(string Emoji)
        {
            if (!string.IsNullOrWhiteSpace(Emoji))
            {
                _db.TechPulseLogs.Add(new TechPulseLog { Emoji = Emoji, Date = DateTime.UtcNow.Date });
                _db.SaveChanges();
            }
            return RedirectToPage();
        }

        public class PulseSummaryDto
        {
            public string Emoji { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }
}
