namespace Services
{
    public interface IDeviceResolver
    {
        string GetDeviceType(HttpContext context);
    }

    public class DeviceResolver : IDeviceResolver
    {
        public string GetDeviceType(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString().ToLowerInvariant();
            if (userAgent.Contains("mobile")) return "mobile";
            if (userAgent.Contains("tablet")) return "tablet";
            return "desktop";
        }
    }
}
