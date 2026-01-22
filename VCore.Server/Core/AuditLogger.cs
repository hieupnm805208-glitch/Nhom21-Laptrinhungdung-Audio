using System;
using System.IO;

namespace VCore.Server.Core
{
    // Lớp ghi log lịch sử hoạt động của Server
    public static class AuditLogger
    {
        // Đường dẫn file log nằm cùng thư mục chạy ứng dụng
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.log");
        private static readonly object _lock = new object();

        // Ghi thông tin sự kiện
        public static void Log(string message, string level = "INFO")
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            
            // Hiển thị ra màn hình Console cho người quản trị thấy
            Console.WriteLine(logEntry);

            // Lưu vào file .log để tra cứu sau này (đảm bảo an toàn luồng bằng lock)
            lock (_lock)
            {
                try
                {
                    File.AppendAllLines(LogFilePath, new[] { logEntry });
                }
                catch
                {
                    // Nếu lỗi file (đang bận...) thì bỏ qua để không làm treo server
                }
            }
        }

        // Ghi lỗi hệ thống
        public static void Error(string message, Exception ex = null)
        {
            Log($"{message} {(ex != null ? $"| Chi tiết lỗi: {ex.Message}" : "")}", "ERROR");
        }
    }
}
