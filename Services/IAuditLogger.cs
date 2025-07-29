namespace MVP_Core.Services
{
    public interface IAuditLogger
    {
        void Log(string message);
    }
}