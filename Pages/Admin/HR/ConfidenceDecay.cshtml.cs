// Sprint 76.1 Patch: PageModel for Employee Confidence Decay logs
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Admin.HR
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
