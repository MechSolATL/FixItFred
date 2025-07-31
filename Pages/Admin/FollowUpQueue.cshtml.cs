using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System;
using Data;
using Data.Models;

namespace Pages.Admin
{
    public class FollowUpQueueModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<FollowUpActionLog> FollowUpLogs { get; set; } = new();
        public List<string> TriggerTypes { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
        public List<string> Sources { get; set; } = new();
        public string? FilterType { get; set; }
        public string? FilterStatus { get; set; }
        public string? FilterSource { get; set; }
        public Dictionary<string, int> TriggerEffectiveness { get; set; } = new();
        public int OpenCount { get; set; }
        public int IgnoredCount { get; set; }
        public FollowUpQueueModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(string? type, string? status, string? source)
        {
            FilterType = type;
            FilterStatus = status;
            FilterSource = source;
            var query = _db.FollowUpActionLogs.AsQueryable();
            if (!string.IsNullOrEmpty(type))
                query = query.Where(l => l.TriggerType == type);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == status);
            if (!string.IsNullOrEmpty(source))
                query = query.Where(l => l.ActionType == source);
            FollowUpLogs = query.OrderByDescending(l => l.TriggeredAt).Take(100).ToList();
            TriggerTypes = _db.FollowUpActionLogs.Select(l => l.TriggerType).Distinct().OrderBy(x => x).ToList();
            Statuses = _db.FollowUpActionLogs.Select(l => l.Status).Distinct().OrderBy(x => x).ToList();
            Sources = _db.FollowUpActionLogs.Select(l => l.ActionType).Distinct().OrderBy(x => x).ToList();
            // Chart: Most Effective Triggers (by ConversionCount)
            TriggerEffectiveness = _db.FollowUpActionLogs
                .GroupBy(l => l.TriggerType)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.ConversionCount ?? 0));
            // Chart: Open vs Ignored
            OpenCount = _db.FollowUpActionLogs.Count(l => l.Status == "Open" || l.Status == "Pending");
            IgnoredCount = _db.FollowUpActionLogs.Count(l => l.Status == "Ignored");
        }
    }
}
