using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Data.Models;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class BehaviorInsightsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly BehaviorPatternEngine _engine;
        private readonly TrustIndexScoringService _trustService;
        public PermissionService PermissionService { get; }
        public AdminUser AdminUser { get; }

        public BehaviorInsightsModel(ApplicationDbContext db, BehaviorPatternEngine engine, TrustIndexScoringService trustService, PermissionService permissionService)
        {
            _db = db;
            _engine = engine;
            _trustService = trustService;
            PermissionService = permissionService;
            AdminUser = HttpContext?.Items["AdminUser"] as AdminUser ?? new AdminUser { EnabledModules = new List<string>() };
        }

        [BindProperty(SupportsGet = true)]
        public int? SelectedTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SelectedPatternType { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public List<ViolationInsightModel> Insights { get; set; } = new();
        public List<string> PatternTypes { get; set; } = Enum.GetNames(typeof(ViolationPatternType)).ToList();
        public List<SelectListItem> TechnicianOptions { get; set; } = new();

        public void OnGet()
        {
            TechnicianOptions = _db.Technicians.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
            var insights = _engine.AnalyzeTechnicianBehavior(SelectedTechnicianId, FromDate, ToDate);
            if (!string.IsNullOrEmpty(SelectedPatternType) && Enum.TryParse<ViolationPatternType>(SelectedPatternType, out var patternType))
                insights = insights.Where(i => i.PatternType == patternType).ToList();
            Insights = insights;
        }

        public int GetTrustIndex(int technicianId)
        {
            return _trustService.GetCachedTrustIndex(technicianId) ?? 100;
        }
    }
}
