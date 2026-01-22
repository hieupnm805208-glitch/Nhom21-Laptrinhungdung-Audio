using System;
using System.IO;

namespace VCore.Server.Core
{
    public static class AuditLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.log");
        private static readonly object _lock = new object();

        public static void Log(string message, string level = "INFO")
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            
            // Console output
            Console.WriteLine(logEntry);

            // File output
            lock (_lock)
            {
                try
                {
                    File.AppendAllLines(LogFilePath, new[] { logEntry });
                }
                catch
                {
                    // Silently fail if file is locked
                }
            }
        }

        public static void Error(string message, Exception ex = null)
        {
            Log($"{message} {(ex != null ? $"| Exception: {ex.Message}" : "")}", "ERROR");
        }
    }
}
