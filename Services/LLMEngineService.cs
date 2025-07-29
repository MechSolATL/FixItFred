// Sprint 90.1
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services
{
    // Sprint 90.1
    public class LLMEngineService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public LLMEngineService(HttpClient httpClient, IConfiguration configuration, ApplicationDbContext db)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _db = db;
        }

        public async Task<string> RunPromptAsync(string prompt)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:DefaultModel"] ?? "gpt-4";

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model,
                messages = new[] {
                    new { role = "user", content = prompt }
                },
                temperature = Convert.ToDouble(_configuration["OpenAI:Temperature"] ?? "0.7"),
                max_tokens = Convert.ToInt32(_configuration["OpenAI:MaxTokens"] ?? "2048")
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message ?? "No response.";
        }

        // Sprint 91.17 - TroubleshootingBrain
        public async Task LogTroubleshootingAttemptAsync(Guid techId, string promptInput, string suggestedFix, bool? wasSuccessful, string techNotes)
        {
            var log = new TroubleshootingAttemptLog
            {
                TechId = techId,
                PromptInput = promptInput,
                SuggestedFix = suggestedFix,
                WasSuccessful = wasSuccessful ?? false,
                TechNotes = techNotes,
                Timestamp = DateTime.UtcNow
            };
            _db.TroubleshootingAttemptLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        // Sprint 91.17 - TroubleshootingBrain
        public async Task PromoteKnownFixAsync(string errorCode, string equipmentType, string manufacturer, string fix)
        {
            var known = await _db.KnownFixes.FirstOrDefaultAsync(k => k.ErrorCode == errorCode && k.EquipmentType == equipmentType && k.Manufacturer == manufacturer);
            if (known == null)
            {
                known = new KnownFix
                {
                    ErrorCode = errorCode,
                    EquipmentType = equipmentType,
                    Manufacturer = manufacturer,
                    CommonFix = fix,
                    SuccessCount = 1,
                    LastConfirmed = DateTime.UtcNow
                };
                _db.KnownFixes.Add(known);
            }
            else
            {
                known.CommonFix = fix;
                known.SuccessCount++;
                known.LastConfirmed = DateTime.UtcNow;
                _db.KnownFixes.Update(known);
            }
            await _db.SaveChangesAsync();
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
