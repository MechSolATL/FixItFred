using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.Analytics;
using Models;
using Data;

namespace Pages.Admin
{
    // Sprint 86.5 — Flow Audit Page
    public class FlowAuditModel : PageModel
    {
        private readonly ActionLogService _actionLogService;
        private readonly ApplicationDbContext _db;
        public FlowAuditModel(ActionLogService actionLogService, ApplicationDbContext db)
        {
            _actionLogService = actionLogService;
            _db = db;
        }
        public List<UserActionLog> Logs { get; set; } = new();
        public List<int> MentorCandidates { get; set; } = new();
        public string? FilterUser { get; set; }
        public string? FilterAction { get; set; }
        public DateTime? FilterDate { get; set; }
        public async Task OnGetAsync(string? user, string? action, DateTime? date)
        {
            var query = _db.UserActionLogs.AsQueryable();
            if (!string.IsNullOrEmpty(user) && int.TryParse(user, out int uid))
                query = query.Where(l => l.AdminUserId == uid);
            if (!string.IsNullOrEmpty(action))
                query = query.Where(l => l.ActionType == action);
            if (date.HasValue)
                query = query.Where(l => l.Timestamp.Date == date.Value.Date);
            Logs = await query.OrderByDescending(l => l.Timestamp).Take(500).ToListAsync();
            MentorCandidates = await _actionLogService.DetectMentorCandidates();
        }
    }
}
