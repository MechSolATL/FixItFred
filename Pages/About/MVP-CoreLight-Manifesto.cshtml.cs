using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pages.About
{
    public class MVP_CoreLight_ManifestoModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "MVP-CoreLight Manifesto - About";
        }
    }
}