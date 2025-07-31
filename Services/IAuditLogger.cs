namespace Services
{
    public interface IAuditLogger
    {
        void Log(string message);
    }
}