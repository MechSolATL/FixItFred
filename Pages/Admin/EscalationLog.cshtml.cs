using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;

namespace Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EscalationLogModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<EscalationLogEntry> Logs { get; set; } = new();
        public List<ScheduleQueue> ScheduleQueues { get; set; } = new();
        public string? FilterTech { get; set; }
        public string? FilterFromString { get; set; }
        public string? FilterToString { get; set; }
        public EscalationLogModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(string? tech, string? from, string? to)
        {
            FilterTech = tech;
            FilterFromString = from;
            FilterToString = to;
            var query = _db.EscalationLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(tech))
            {
                var techIds = _db.ScheduleQueues.Where(q => q.AssignedTechnicianName.Contains(tech)).Select(q => q.Id).ToList();
                query = query.Where(e => techIds.Contains(e.ScheduleQueueId));
            }
            if (DateTime.TryParse(from, out var fromDate))
                query = query.Where(e => e.CreatedAt >= fromDate);
            if (DateTime.TryParse(to, out var toDate))
                query = query.Where(e => e.CreatedAt <= toDate.AddDays(1));
            Logs = query.OrderByDescending(e => e.CreatedAt).Take(200).ToList();
            ScheduleQueues = _db.ScheduleQueues.ToList();
        }
    }
}
