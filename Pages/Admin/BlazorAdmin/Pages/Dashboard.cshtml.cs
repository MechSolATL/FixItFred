using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin.BlazorAdmin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public DashboardModel(ApplicationDbContext db, ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _db = db;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        public List<ServiceRequest>? ServiceRequests { get; set; }
        public string ActiveTab { get; set; } = "requests";

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Dashboard");
            ViewData["Title"] = seo?.Title ?? "Admin Dashboard";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            if (Request.Query.ContainsKey("tab"))
            {
                var tab = Request.Query["tab"].ToString();
                if (!string.IsNullOrWhiteSpace(tab))
                    ActiveTab = tab;
            }

            if (ActiveTab == "requests")
            {
                ServiceRequests = _db.ServiceRequests
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(25)
                    .ToList();
            }
        }
    }
}
