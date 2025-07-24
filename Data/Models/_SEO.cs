namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents the SeoMeta metadata for a specific page in the application.
    /// Adheres to SeoMeta best practices such as title length, description length, and robots meta settings.
    /// </summary>
    public class SeoMeta
    {
        /// <summary>
        /// Gets or sets the unique identifier for the SeoMeta record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the page.
        /// Displayed in the browser tab and search engine results.
        /// Recommended: 50–60 characters.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(60, ErrorMessage = "Title should not exceed 60 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meta description of the page.
        /// Shown in search engine snippets.
        /// Recommended: 150–160 characters.
        /// </summary>
        [Required(ErrorMessage = "Meta description is required.")]
        [MaxLength(160, ErrorMessage = "Meta description should not exceed 160 characters.")]
        public string MetaDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meta keywords for the page.
        /// Comma-separated values for legacy SeoMeta systems.
        /// Recommended max: 255 characters.
        /// </summary>
        [MaxLength(255, ErrorMessage = "Keywords should not exceed 255 characters.")]
        public string Keywords { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the page this SeoMeta record is for.
        /// This must match the page's route or identifier exactly.
        /// </summary>
        [Required(ErrorMessage = "Page name is required.")]
        public string PageName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the robots meta tag.
        /// Determines how search engines crawl or index this page.
        /// Default: "index, follow"
        /// </summary>
        [MaxLength(25, ErrorMessage = "Robots meta tag should not exceed 25 characters.")]
        public string Robots { get; set; } = "index, follow";
    }
}
