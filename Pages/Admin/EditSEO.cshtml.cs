using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Helpers;
using Data;
using Data.Models.Seo;
using Services;

namespace Pages.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    [ValidateAntiForgeryToken]
    public class EditSEOModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISeoService _seoService;
        private readonly SessionTracker _session;
        private readonly IDeviceResolver _deviceResolver;

        public EditSEOModel(ApplicationDbContext dbContext, ISeoService seoService, SessionTracker session, IDeviceResolver deviceResolver)
        {
            _dbContext = dbContext;
            _seoService = seoService;
            _session = session;
            _deviceResolver = deviceResolver;
        }

        [BindProperty]
        public SeoMeta SeoMeta { get; set; } = new SeoMeta();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ViewData["Title"] = "Edit SEO";
            var meta = await _seoService.GetSeoByPageNameAsync("EditSEO");
            ViewData["MetaDescription"] = meta?.MetaDescription;
            ViewData["Keywords"] = meta?.Keywords;
            ViewData["Robots"] = meta?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            SeoMeta? found = _dbContext.SEOs.FirstOrDefault(s => s.Id == id);
            if (found == null)
            {
                TempData["Error"] = "SeoMeta entry not found.";
                return RedirectToPage("/Admin/ManageSEO");
            }
            SeoMeta = found;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            SeoMeta? existing = await _dbContext.SEOs.FindAsync(SeoMeta.Id);
            if (existing == null)
            {
                TempData["Error"] = "SeoMeta entry not found.";
                return RedirectToPage("/Admin/ManageSEO");
            }
            existing.Title = SeoMeta.Title;
            existing.MetaDescription = SeoMeta.MetaDescription;
            existing.Keywords = SeoMeta.Keywords;
            existing.Robots = SeoMeta.Robots;
            existing.PageName = SeoMeta.PageName;
            _ = await _dbContext.SaveChangesAsync();
            TempData["Message"] = "✅ SeoMeta entry updated successfully!";
            return RedirectToPage("/Admin/ManageSEO");
        }
    }
}
