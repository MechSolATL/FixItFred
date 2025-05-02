using Microsoft.AspNetCore.Mvc;

namespace MVP_Core.Pages.Profile
{
    public class AuditLogger : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
