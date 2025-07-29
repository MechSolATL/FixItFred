// Sprint 90.1
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    // Sprint 90.1
    public class PromptsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly LLMEngineService _llmEngineService;
        private readonly PromptDebugService _debugService;
        public PromptsModel(ApplicationDbContext db, LLMEngineService llmEngineService, PromptDebugService debugService)
        {
            _db = db;
            _llmEngineService = llmEngineService;
            _debugService = debugService;
        }
        public List<PromptVersion> PromptVersions { get; set; } = new();
        public List<LLMModelProvider> Providers { get; set; } = new();
        [BindProperty]
        public string? PromptInput { get; set; }
        [BindProperty]
        public int? SelectedPromptVersionId { get; set; }
        [BindProperty]
        public int? SelectedProviderId { get; set; }
        [BindProperty]
        public string? ApiKey { get; set; }
        public string? Output { get; set; }
        public async Task OnGetAsync()
        {
            PromptVersions = _db.PromptVersions.ToList();
            Providers = _db.LLMModelProviders.ToList();
        }
        public async Task<IActionResult> OnPostRunPromptAsync()
        {
            if (SelectedPromptVersionId == null || SelectedProviderId == null || string.IsNullOrEmpty(ApiKey))
            {
                await OnGetAsync();
                return Page();
            }
            var promptVersion = _db.PromptVersions.FirstOrDefault(p => p.Id == SelectedPromptVersionId);
            var provider = _db.LLMModelProviders.FirstOrDefault(p => p.Id == SelectedProviderId);
            if (promptVersion == null || provider == null)
            {
                await OnGetAsync();
                return Page();
            }
            Output = await _llmEngineService.RunPromptAsync(provider.ProviderName, provider.ModelName, promptVersion.PromptText, ApiKey!);
            // Log trace
            var trace = new PromptTraceLog
            {
                PromptVersionId = promptVersion.Id,
                UserId = User?.Identity?.Name ?? "Unknown",
                SessionId = HttpContext.Session.Id,
                Input = promptVersion.PromptText,
                Output = Output,
                CreatedAt = DateTime.UtcNow
            };
            await _debugService.LogTraceAsync(trace);
            await OnGetAsync();
            return Page();
        }
    }
}
