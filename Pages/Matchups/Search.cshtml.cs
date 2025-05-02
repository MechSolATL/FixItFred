// ===============================
// File: Pages/Matchups/Search.cshtml.cs
// ===============================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Matchups
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public SearchModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public string Query { get; set; } = string.Empty;

        public List<HeatPumpMatchup> Results { get; set; } = new();

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Search Heat Pump Matchups";
            ViewData["Layout"] = "/Pages/Shared/_Layout.cshtml";

            var seo = await _db.SEOs.FirstOrDefaultAsync(s => s.PageName == "MatchupsSearch");
            if (seo != null)
            {
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = "index, follow"; // Update as needed
            }

            if (!string.IsNullOrWhiteSpace(Query))
            {
                string lower = Query.ToLower();
                Results = await _db.HeatPumpMatchups
                    .Where(m =>
                        m.Brand.ToLower().Contains(lower) ||
                        m.SystemType.ToLower().Contains(lower) ||
                        m.ACoilModel.ToLower().Contains(lower) ||
                        m.OutdoorUnitModel.ToLower().Contains(lower) ||
                        m.FurnaceModel.ToLower().Contains(lower) ||
                        m.SEERRating.ToLower().Contains(lower) ||
                        m.AHRICode.ToLower().Contains(lower) ||
                        m.Notes.ToLower().Contains(lower))
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
        }
    }
}
