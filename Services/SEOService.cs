namespace MVP_Core.Services
{
    public class SeoService : ISeoService
    {
        public Task<SeoMeta> GetSeoMetaAsync(string pageName)
        {
            return Task.FromResult(new SeoMeta
            {
                PageName = pageName,
                Title = "Sample Title",
                MetaDescription = "Default SeoMeta description",
                Keywords = "Keyword1, Keyword2",
                Robots = "index, follow"
            });
        }

        public Task<SeoMeta> GetSeoByPageNameAsync(string pageName)
        {
            return GetSeoMetaAsync(pageName); // Aliased version
        }
    }
}
