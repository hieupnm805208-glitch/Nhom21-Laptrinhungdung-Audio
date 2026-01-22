using System;
using System.Threading.Tasks;
using VCore.Server.Core;

namespace VCore.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Thiết lập tiêu đề cửa sổ
            Console.Title = "Nhóm 21 - V-CORE Messenger Server";
            
            // Cổng mặc định của ứng dụng
            int port = 8080;
            SocketServer server = new SocketServer(port);
            
            Console.WriteLine("==================================================");
            Console.WriteLine("   HỆ THỐNG MÁY CHỦ V-CORE MESSENGER (NHÓM 21)   ");
            Console.WriteLine("   KIẾN TRÚC: TCP SOCKET BẤT ĐỒNG BỘ             ");
            Console.WriteLine("==================================================");
            
            // Bắt đầu lắng nghe kết nối
            await server.StartAsync();
        }
    }
}
