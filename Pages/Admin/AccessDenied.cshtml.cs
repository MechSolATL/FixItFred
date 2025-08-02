using Data.Models;

namespace Pages.Admin
{
    public class AccessDeniedModel : PageModel
    {
        // [Sprint1002_FixItFred] Fixed to use correct SeoMeta class
        public SeoMeta Seo { get; set; } = new SeoMeta();

        public string Title => Seo.Title;
        public string MetaDescription => Seo.MetaDescription;
        public string Keywords => Seo.Keywords;
        public string Robots => Seo.Robots;

        public void OnGet()
        {
            // ...existing code...
        }
    }
}