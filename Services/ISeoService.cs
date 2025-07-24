namespace MVP_Core.Services
{
    public interface ISeoService
    {
        Task<SeoMeta> GetSeoMetaAsync(string pageName);
        Task<SeoMeta> GetSeoByPageNameAsync(string pageName);
    }
}
