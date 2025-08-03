using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pages.About
{
    public class MVP_Core_ManifestoModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "MVP-Core Manifesto";
        }
    }
}