using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace MVP_Core.Pages.Admin
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
