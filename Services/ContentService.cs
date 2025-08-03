namespace MVP_Core.Services
{
    public class ContentService : IContentService
    {
        public async Task<string> GetByKeyAsync(string key)
        {
            // Mock implementation
            return await Task.FromResult($"Content for key: {key}");
        }

        public async Task<string> GetContentAsync(string key)
        {
            // Mock implementation
            return await Task.FromResult($"Content for: {key}");
        }
    }
}
