using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Services.Admin;

namespace Pages.Admin
{
    public class SyncAuditModel : PageModel
    {
        public SyncAnalyticsService SyncAnalyticsService { get; set; }
        public void OnGet()
        {
            SyncAnalyticsService = HttpContext.RequestServices.GetRequiredService<SyncAnalyticsService>();
        }
    }
}
