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
        private readonly TechnicianPatternProfilerService _profilerService; // Sprint 79.3: CS8618/CS860X warning cleanup
        public PatternProfilerModel(TechnicianPatternProfilerService profilerService)
        {
            _profilerService = profilerService ?? throw new System.ArgumentNullException(nameof(profilerService)); // Sprint 79.3: CS8618/CS860X warning cleanup
            Patterns = new List<TechnicianPatternProfile>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            Annotations = new Dictionary<int, string>(); // Sprint 79.3: CS8618/CS860X warning cleanup
            SelectedRiskLevel = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
            SelectedPatternType = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        }

        [BindProperty]
        public string SelectedRiskLevel { get; set; } = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public string SelectedPatternType { get; set; } = string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup
        public List<TechnicianPatternProfile> Patterns { get; set; } = new List<TechnicianPatternProfile>(); // Sprint 79.3: CS8618/CS860X warning cleanup
        [BindProperty]
        public Dictionary<int, string> Annotations { get; set; } = new Dictionary<int, string>(); // Sprint 79.3: CS8618/CS860X warning cleanup

        public string GetAnnotation(int id) => Annotations.ContainsKey(id) ? Annotations[id] : string.Empty; // Sprint 79.3: CS8618/CS860X warning cleanup

        public async Task OnGetAsync()
        {
            Patterns = await _profilerService.AnalyzeEscalationPatternsAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
        }

        public async Task<IActionResult> OnPostRunAnalysisAsync()
        {
            var allPatterns = await _profilerService.AnalyzeEscalationPatternsAsync(); // Sprint 79.3: CS8618/CS860X warning cleanup
            Patterns = allPatterns
                .Where(p => (string.IsNullOrEmpty(SelectedRiskLevel) || p.RiskLevel == SelectedRiskLevel)
                         && (string.IsNullOrEmpty(SelectedPatternType) || p.PatternType == SelectedPatternType))
                .ToList(); // Sprint 79.3: CS8618/CS860X warning cleanup
            return Page();
        }
    }
}
