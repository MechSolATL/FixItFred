using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System;

namespace MVP_Core.Helpers
{
    public static class SessionHelper
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            if (value == null) return;
            session.SetString(key, JsonSerializer.Serialize(value, _serializerOptions));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value, _serializerOptions);
            }
            catch (JsonException)
            {
                // Optional: log deserialization error if desired
                // Example: Logger.LogWarning($"Failed to deserialize session key: {key}");

                return default;
            }
        }
    }
}
