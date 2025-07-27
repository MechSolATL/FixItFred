// Sprint 76.1 Patch: PageModel for Ego Vector Logs admin review
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin.HR
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
