// Sprint 90.1
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data;
using Services;

namespace Pages.Admin
{
    // Sprint 90.1
    public class PromptsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly LLMEngineService _llmEngineService;
        public PromptsModel(ApplicationDbContext db, LLMEngineService llmEngineService)
        {
            _db = db;
            _llmEngineService = llmEngineService;
        }
        public List<PromptVersion> PromptVersions { get; set; } = new();
        [BindProperty]
        public string? PromptInput { get; set; }
        public string? Output { get; set; }
        public SeoMeta Seo { get; set; } = new SeoMeta();
        public string TierStatus { get; set; } = "Basic";
        public string ViewTitle { get; set; } = "Prompts";
        public string? ReturnUrl { get; set; }
        public async Task OnGetAsync()
        {
            PromptVersions = _db.PromptVersions.ToList();
        }
        public async Task<IActionResult> OnPostRunPromptAsync()
        {
            if (string.IsNullOrEmpty(PromptInput))
            {
                await OnGetAsync();
                return Page();
            }
            Output = await _llmEngineService.RunPromptAsync(PromptInput);
            await OnGetAsync();
            return Page();
        }
    }
}
