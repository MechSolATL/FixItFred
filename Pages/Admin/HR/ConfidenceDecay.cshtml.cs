// Sprint 76.1 Patch: PageModel for Employee Confidence Decay logs
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin.HR
{
    public class ConfidenceDecayModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ConfidenceDecayModel(ApplicationDbContext db) { _db = db; }
        public List<EmployeeConfidenceDecayLog> EmployeeConfidenceDecayLogs { get; set; } = new();
        public void OnGet()
        {
            EmployeeConfidenceDecayLogs = _db.EmployeeConfidenceDecayLogs.OrderByDescending(e => e.WeekStart).Take(50).ToList();
        }
    }
}
