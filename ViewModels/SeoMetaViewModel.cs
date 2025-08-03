using System.ComponentModel.DataAnnotations;

namespace MVP_Core.ViewModels
{
    /// <summary>
    /// View model for SEO metadata display in _SEOHead.cshtml
    /// [Sprint91_27] Added for proper SEO integration with trace IDs
    /// </summary>
    public class SeoMetaViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public string? Robots { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? OgImage { get; set; }
        public string? TraceId { get; set; }
        public string? PagePath { get; set; }
    }
}