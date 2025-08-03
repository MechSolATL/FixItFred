using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Services;

namespace Pages.Admin.BlazorAdmin.Pages
{
    public class AuditLogModel : PageModel
    {
        private readonly ISeoService _seoService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuditLogModel(ISeoService seoService, IHttpClientFactory httpClientFactory)
        {
            _seoService = seoService;
            _httpClientFactory = httpClientFactory;
        }

        public AuditLogDataModel? AuditLogData { get; set; }
        public bool Loading { get; set; } = true;

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoMetaAsync("AuditLog");
            ViewData["Title"] = seo?.Title ?? "Audit Log";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = await client.GetStringAsync("api/auditlog");
                AuditLogData = JsonSerializer.Deserialize<AuditLogDataModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                AuditLogData = null;
            }
            Loading = false;
        }

        public async Task<IActionResult> OnGetJsonAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var json = await client.GetStringAsync("api/auditlog");
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "solace-auditlog.json");
        }

        public class AuditLogDataModel
        {
            public DateTime GeneratedAt { get; set; }
            public List<AuditSection> Sections { get; set; } = new();
            public AuditSignature Signature { get; set; } = new();
        }
        public class AuditSection
        {
            public string Section { get; set; } = "";
            public List<AuditFile> Files { get; set; } = new();
        }
        public class AuditFile
        {
            public string Filename { get; set; } = "";
            public DateTime Timestamp { get; set; }
            public List<string> ComplianceFixes { get; set; } = new();
        }
        public class AuditSignature
        {
            public string PreparedFor { get; set; } = "";
            public string PreparedBy { get; set; } = "";
            public string Date { get; set; } = "";
        }
    }
}
