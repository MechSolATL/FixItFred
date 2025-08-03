namespace MVP_Core.Services
{
    public interface IContentService
    {
        Task<string> GetByKeyAsync(string key);
        Task<string> GetContentAsync(string key);
    }
}
