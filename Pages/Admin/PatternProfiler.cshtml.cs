using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class PatternProfilerModel : PageModel
    {
        private readonly TechnicianPatternProfilerService _profilerService;
        public PatternProfilerModel(TechnicianPatternProfilerService profilerService)
        {
            _profilerService = profilerService;
            Patterns = new List<TechnicianPatternProfile>();
            Annotations = new Dictionary<int, string>();
        }

        [BindProperty]
        public string SelectedRiskLevel { get; set; }
        [BindProperty]
        public string SelectedPatternType { get; set; }
        public List<TechnicianPatternProfile> Patterns { get; set; }
        [BindProperty]
        public Dictionary<int, string> Annotations { get; set; }

        public string GetAnnotation(int id) => Annotations.ContainsKey(id) ? Annotations[id] : "";

        public async Task OnGetAsync()
        {
            Patterns = await _profilerService.AnalyzeEscalationPatternsAsync();
        }

        public async Task<IActionResult> OnPostRunAnalysisAsync()
        {
            var allPatterns = await _profilerService.AnalyzeEscalationPatternsAsync();
            Patterns = allPatterns
                .Where(p => (string.IsNullOrEmpty(SelectedRiskLevel) || p.RiskLevel == SelectedRiskLevel)
                         && (string.IsNullOrEmpty(SelectedPatternType) || p.PatternType == SelectedPatternType))
                .ToList();
            return Page();
        }
    }
}
