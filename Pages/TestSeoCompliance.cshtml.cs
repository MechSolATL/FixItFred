// Created by FixItFred
// Timestamp: 2025-07-30  ?? Patch Sprint 91.1
// Purpose: Scaffolded PageModel for TestSeoCompliance Razor Page
// Logic Summary:
// - Provides OnGetAsync method for injecting SEO metadata dynamically
// - Retrieves SEO details from the database by page name "TestSeoCompliance"
// - Assigns SEO values to ViewData for Razor access
// - Ensures compliance with Service-Atlanta SEO and layout protocols

using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Models;
using Services;

namespace Pages
{
    public class TestSeoComplianceModel : PageModel
    {
        private readonly ISeoService _seoService;

        public TestSeoComplianceModel(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("TestSeoCompliance");
            if (seo != null)
            {
                ViewData["Title"] = seo.Title;
                ViewData["MetaDescription"] = seo.MetaDescription;
                ViewData["Keywords"] = seo.Keywords;
                ViewData["Robots"] = seo.Robots;
            }
        }
    }
}