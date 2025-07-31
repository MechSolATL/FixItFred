using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Data;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class AwardCenterModel : PageModel
    {
        private readonly TechnicianAwardService _awardService;
        private readonly ApplicationDbContext _db;
        public AwardCenterModel(TechnicianAwardService awardService, ApplicationDbContext db)
        {
            _awardService = awardService;
            _db = db;
        }
        public List<TechnicianAwardLog> Awards { get; set; } = new();
        public List<Data.Models.Technician> Technicians { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterAwardType { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public DateTime? FilterFrom { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? FilterTo { get; set; }
        [BindProperty]
        public int NewTechnicianId { get; set; }
        [BindProperty]
        public string NewAwardType { get; set; } = string.Empty;
        [BindProperty]
        public string? NewReason { get; set; }
        [BindProperty]
        public string NewIssuer { get; set; } = string.Empty;
        [BindProperty]
        public int NewTrustBoost { get; set; }
        [BindProperty]
        public int NewKarmaBoost { get; set; }
        [BindProperty]
        public string? NewAwardLevel { get; set; }
        public async Task OnGetAsync()
        {
            Technicians = await _db.Technicians.OrderBy(t => t.FullName).ToListAsync();
            Awards = await _awardService.GetAwardsAsync(FilterTechnicianId, FilterFrom, FilterTo, FilterAwardType);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (NewTechnicianId > 0 && !string.IsNullOrWhiteSpace(NewAwardType) && !string.IsNullOrWhiteSpace(NewIssuer))
            {
                await _awardService.IssueAwardAsync(NewTechnicianId, NewAwardType, NewReason, NewIssuer, NewTrustBoost, NewKarmaBoost, NewAwardLevel);
            }
            return RedirectToPage(new { TechnicianId = FilterTechnicianId, AwardType = FilterAwardType, From = FilterFrom, To = FilterTo });
        }
    }
}
