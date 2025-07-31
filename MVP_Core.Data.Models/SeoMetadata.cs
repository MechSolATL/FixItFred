namespace MVP_Core.Data.Models
{
    public class SeoMetadata
    {
        public string Title { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string Robots { get; set; } = "index,follow";
    }
}
