using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    /// <summary>
    /// Represents the SEO metadata for a specific page in the application.
    /// Adheres to SEO best practices such as title length, description length, and robots meta settings.
    /// </summary>
    public class SEO
    {
        /// <summary>
        /// Gets or sets the unique identifier for the SEO record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the page. 
        /// The title is displayed in the browser tab and must be concise for SEO optimization.
        /// Recommended: 50-60 characters.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(60, ErrorMessage = "Title should not exceed 60 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meta description of the page. 
        /// Search engines display this in search results as a page summary.
        /// Recommended: 150-160 characters.
        /// </summary>
        [Required(ErrorMessage = "Meta description is required.")]
        [MaxLength(160, ErrorMessage = "Meta description should not exceed 160 characters.")]
        public string MetaDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the keywords for the page. 
        /// Keywords should be comma-separated for SEO optimization.
        /// Recommended: Max 255 characters.
        /// </summary>
        [MaxLength(255, ErrorMessage = "Keywords should not exceed 255 characters.")]
        public string Keywords { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the page to which this SEO data applies.
        /// PageName must match the page route or identifier.
        /// </summary>
        [Required(ErrorMessage = "Page name is required.")]
        public string PageName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the robots meta tag for the page. 
        /// Controls how search engines index and follow the page links.
        /// Common values:
        /// - "index, follow" (Default): Allow indexing and crawling.
        /// - "noindex, nofollow": Prevent indexing and crawling.
        /// - "noindex, follow": Prevent indexing but allow crawling.
        /// </summary>
        [MaxLength(25, ErrorMessage = "Robots meta tag should not exceed 25 characters.")]
        public string RobotsMeta { get; set; } = "index, follow";
    }
}
