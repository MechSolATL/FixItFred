namespace Helpers
{
    public static class HealthStatusHelper
    {
        public static string GetStatusLevel(int errorCount, int warningCount)
        {
            if (errorCount > 0) return "Error";
            if (warningCount > 0) return "Warning";
            return "OK";
        }
    }
}
