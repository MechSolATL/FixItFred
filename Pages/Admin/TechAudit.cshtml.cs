using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Admin
{
    public class TechAuditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly TechnicianAuditService _auditService;
        public TechAuditModel(ApplicationDbContext db)
        {
            _db = db;
            _auditService = new TechnicianAuditService(db);
        }

        [BindProperty(SupportsGet = true)]
        public int? TechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }
        [BindProperty(SupportsGet = true)]
        public TechnicianAuditActionType? ActionType { get; set; }

        public List<TechnicianAuditLog> Logs { get; set; } = new();
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            if (TechnicianId.HasValue && Date.HasValue && ActionType.HasValue)
            {
                Logs = await _auditService.GetLogsByTechAndDateAsync(TechnicianId.Value, Date.Value);
                Logs = Logs.Where(l => l.ActionType == ActionType.Value).ToList();
            }
            else if (TechnicianId.HasValue && Date.HasValue)
            {
                Logs = await _auditService.GetLogsByTechAndDateAsync(TechnicianId.Value, Date.Value);
            }
            else if (TechnicianId.HasValue)
            {
                Logs = await _auditService.GetLogsByRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1), TechnicianId);
            }
            else if (Date.HasValue)
            {
                Logs = await _auditService.GetLogsByRangeAsync(Date.Value.Date, Date.Value.Date.AddDays(1));
            }
            else if (ActionType.HasValue)
            {
                Logs = await _auditService.GetLogsByActionTypeAsync(ActionType.Value);
            }
            else
            {
                Logs = await _auditService.GetLogsByRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));
            }
        }

        public string GetTechnicianName(int technicianId)
        {
            var tech = Technicians.FirstOrDefault(t => t.Id == technicianId);
            return tech?.FullName ?? $"Tech #{technicianId}";
        }
    }
}
