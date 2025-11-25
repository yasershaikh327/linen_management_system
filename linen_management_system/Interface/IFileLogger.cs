namespace linen_management_system.Interface
{
    public interface IFileLogger
    {
        void LogInfo(string message);
        void LogError(string message, Exception ex);
    }
}
