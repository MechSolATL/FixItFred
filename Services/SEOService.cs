using Data.Models.Seo;

namespace Services
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

        public Task<string> GetSeoByPageNameAsync(string pageName)
        {
            return Task.FromResult($"SEO Meta for {pageName}");
        }

        public async Task<string> GetSeoForPageAsync(string pageName)
        {
            // Placeholder implementation
            return await Task.FromResult("Default SEO");
        }
    }
}
