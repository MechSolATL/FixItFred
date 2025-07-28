using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class TechAuditModel : PageModel
    {
        private readonly MVP_Core.Services.Admin.TechnicianAuditService _auditService;
        internal readonly ApplicationDbContext _db;
        public TechAuditModel(ApplicationDbContext db, MVP_Core.Services.Admin.TechnicianAuditService auditService)
        {
            _db = db;
            _auditService = auditService;
        }

        [BindProperty(SupportsGet = true)]
        public int? TechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }
        [BindProperty(SupportsGet = true)]
        public TechnicianAuditActionType? ActionType { get; set; }

        // FixItFred: Use TechnicianBehaviorLog for logs to match TechnicianAuditService
        public List<MVP_Core.Models.TechnicianBehaviorLog> Logs { get; set; } = new();
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            if (TechnicianId.HasValue && Date.HasValue && ActionType.HasValue)
            {
                Logs = await _auditService.GetLogsByTechAndDateAsync(TechnicianId.Value, Date.Value);
                // No ActionType filter for stub, keep as is for now
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
                Logs = await _auditService.GetLogsByActionTypeAsync(ActionType.Value.ToString());
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
