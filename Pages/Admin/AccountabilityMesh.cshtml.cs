using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data;

namespace Pages.Admin
{
    public class AccountabilityMeshModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<AccountabilityDelayLog> MeshLogs { get; set; } = new();

        public AccountabilityMeshModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            MeshLogs = _db.AccountabilityDelayLogs.OrderByDescending(x => x.CreatedAt).ToList();
        }
    }
}
