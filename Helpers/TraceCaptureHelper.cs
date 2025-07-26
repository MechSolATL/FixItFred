using System.Text.Json;

namespace Helpers
{
    public static class TraceCaptureHelper
    {
        public static string CaptureContext(object data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = false,
#if NET8_0_OR_GREATER
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
#else
                IgnoreNullValues = true,
#endif
            });
            // Optional: redact known sensitive fields
            return json.Replace("password", "***").Replace("email", "***");
        }
    }
}
