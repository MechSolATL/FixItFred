// Sprint 76.1 Patch: PageModel for Ego Vector Logs admin review
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Admin.HR
{
    public class EgoVectorsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EgoVectorsModel(ApplicationDbContext db) { _db = db; }
        public List<EgoVectorLog> EgoVectorLogs { get; set; } = new();
        public void OnGet()
        {
            EgoVectorLogs = _db.EgoVectorLogs.OrderByDescending(e => e.WeekStart).Take(50).ToList();
        }
    }
}
