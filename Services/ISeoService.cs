using Data.Models.Seo;

namespace Services
{
    public interface ISeoService
    {
        Task<string> GetSeoForPageAsync(string pageName);
        Task<string> GetSeoByPageNameAsync(string pageName);
        Task<SeoMeta> GetSeoMetaAsync(string pageName);
    }
}
