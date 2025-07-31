using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Data;
using Data.Models;

namespace Pages.Admin
{
    public class ReferralsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ReferralsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<ReferralCode> Codes { get; set; } = new();
        public void OnGet(string? search)
        {
            Codes = string.IsNullOrWhiteSpace(search)
                ? _db.ReferralCodes.OrderByDescending(r => r.CreatedAt).Take(50).ToList()
                : _db.ReferralCodes.Where(r => r.Code.Contains(search)).ToList();
        }
        public IActionResult OnPostFlag(int id)
        {
            var code = _db.ReferralCodes.Find(id);
            if (code != null)
            {
                code.FraudFlagLevel = 2;
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
        public FileResult OnGetExport()
        {
            var codes = _db.ReferralCodes.ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Code,Owner,Created,Usage,FraudFlag");
            foreach (var c in codes)
            {
                var owner = _db.Customers.FirstOrDefault(x => x.Id == c.OwnerCustomerId)?.Name ?? "Unknown";
                sb.AppendLine($"{c.Code},{owner},{c.CreatedAt},{c.UsageCount},{c.FraudFlagLevel}");
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "referrals.csv");
        }
    }
}
