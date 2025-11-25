using linen_management_system.Interface;

namespace LinenManagement.Services
{

    public class FileLogger : IFileLogger
    {
        private readonly string _logFolder = "Logs";

        public FileLogger()
        {
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogError(string message, Exception ex)
        {
            string error = $"{message} | Exception: {ex.Message}";
            WriteLog("ERROR", error);
        }

        private void WriteLog(string level, string message)
        {
            string fileName = $"log-{DateTime.Now:yyyy-MM-dd}.txt";
            string fullPath = Path.Combine(_logFolder, fileName);
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            File.AppendAllText(fullPath, logEntry + Environment.NewLine);
        }
    }
}