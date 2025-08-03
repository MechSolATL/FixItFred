using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pages.MVP_CoreLight
{
    public class ManifestoModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "MVP-CoreLight Manifesto";
        }
    }
}