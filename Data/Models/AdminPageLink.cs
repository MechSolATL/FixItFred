namespace MVP_Core.Data.Models
{
    public class AdminPageLink
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsExternal { get; set; }

        public AdminPageLink(string title, string url, string description, bool isExternal = false)
        {
            Title = title;
            Url = url;
            Description = description;
            IsExternal = isExternal;
        }
    }
}
