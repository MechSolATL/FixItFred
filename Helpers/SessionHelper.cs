using System.Text.Json;

namespace Helpers
{
    public static class SessionHelper
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            if (value == null)
            {
                return;
            }

            session.SetString(key, JsonSerializer.Serialize(value, _serializerOptions));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            string? value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

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
