using Data.Models;

namespace Pages.Admin
{
    public class AccessDeniedModel : PageModel
    {
        public SeoMetadata Seo { get; set; } = new SeoMetadata();

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