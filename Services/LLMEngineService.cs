// Sprint 90.1
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System;
using MVP_Core.Data.Models;

namespace MVP_Core.Services
{
    // Sprint 90.1
    public class LLMEngineService
    {
        private readonly HttpClient _httpClient;
        public LLMEngineService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> RunPromptAsync(string provider, string model, string prompt, string apiKey)
        {
            // Example: OpenAI/Volcengine call stub
            var requestBody = new
            {
                model = model,
                prompt = prompt
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            // This is a stub. Replace with actual endpoint logic per provider.
            var url = provider == "OpenAI" ? "https://api.openai.com/v1/completions" : "https://api.volcengine.com/llm/generate";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }

    // Sprint 90.1
    public class PromptDebugService
    {
        private readonly ApplicationDbContext _db;
        public PromptDebugService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task LogTraceAsync(PromptTraceLog trace)
        {
            _db.PromptTraceLogs.Add(trace);
            await _db.SaveChangesAsync();
        }
        // Add more test/debug logic as needed
    }
}
