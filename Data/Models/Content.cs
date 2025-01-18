namespace MVP_Core.Data.Models
{
    public class Content
    {
        public int Id { get; set; }
        public string PageName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string ContentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
