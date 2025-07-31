using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;
using Data;

namespace Pages.Admin
{
    public class SetupWizardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public SetupWizardModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        [BindProperty]
        [Required, EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;
        [BindProperty]
        [Required]
        public string Tier { get; set; } = "Starter";
        [BindProperty]
        public string LogoUrl { get; set; } = string.Empty;
        [BindProperty]
        public string PrimaryColor { get; set; } = "#007bff";
        [BindProperty]
        public FeatureTogglesModel FeatureToggles { get; set; } = new();
        [BindProperty]
        public int Step { get; set; } = 0;
        public bool Success { get; set; } = false;
        private Guid _createdTenantId;

        public class FeatureTogglesModel
        {
            public bool TechnicianTracking { get; set; } = true;
            public bool AccountingView { get; set; } = false;
            public bool CommunityFeed { get; set; } = false;
            public bool HeatPumpMatchups { get; set; } = false;
            public override string ToString() => string.Join(", ",
                TechnicianTracking ? "TechnicianTracking" : null,
                AccountingView ? "AccountingView" : null,
                CommunityFeed ? "CommunityFeed" : null,
                HeatPumpMatchups ? "HeatPumpMatchups" : null
            ).Replace(", ,", ",").Trim(',');
        }

        public void OnGet()
        {
            // Defaults
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (!ModelState.IsValid && action != "Back")
                return Page();
            if (action == "Back")
            {
                Step = Math.Max(0, Step - 1);
                return Page();
            }
            if (Step < 4)
            {
                Step++;
                return Page();
            }
            // Final step: Save
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = CompanyName,
                ContactEmail = ContactEmail,
                Tier = Tier,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _db.Tenants.AddAsync(tenant);
            await _db.SaveChangesAsync();
            var branding = new TenantBranding
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                LogoUrl = LogoUrl,
                FaviconUrl = string.Empty,
                PrimaryColor = PrimaryColor,
                SecondaryColor = "#ffffff"
            };
            await _db.TenantBrandings.AddAsync(branding);
            var settings = new TenantSettings
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                EnableBilling = Tier != "Starter",
                EnableGratitudeWall = Tier == "Enterprise",
                EnableCustomDomains = Tier == "Enterprise"
            };
            await _db.TenantSettings.AddAsync(settings);
            var modules = new TenantModules
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                TechnicianTracking = FeatureToggles.TechnicianTracking,
                AccountingView = FeatureToggles.AccountingView,
                CommunityFeed = FeatureToggles.CommunityFeed,
                HeatPumpMatchups = FeatureToggles.HeatPumpMatchups
            };
            await _db.TenantModules.AddAsync(modules);
            await _db.SaveChangesAsync();
            Success = true;
            _createdTenantId = tenant.Id;
            // Redirect after short delay
            Response.Headers["Refresh"] = "2;url=/Admin/TenantDashboard?tenantId=" + tenant.Id;
            return Page();
        }
    }
}
