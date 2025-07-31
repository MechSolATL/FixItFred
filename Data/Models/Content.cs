namespace Data.Models
{
    /// <summary>
    /// Represents dynamic content blocks used throughout the site.
    /// Supports banners, taglines, carousels, and HTML/text sections per Razor Page.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Unique identifier for the content block.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The logical name of the page this content is tied to (e.g., "Services/Plumbing").
        /// </summary>
        public string PageName { get; set; } = string.Empty;

        /// <summary>
        /// The section of the page (e.g., "Carousel1", "MarketingText", "BannerTop").
        /// </summary>
        public string Section { get; set; } = string.Empty;

        /// <summary>
        /// The actual content (can be image filename, HTML, or raw text).
        /// </summary>
        public string ContentText { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp for when this content was first created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp for the most recent content update, if applicable.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
