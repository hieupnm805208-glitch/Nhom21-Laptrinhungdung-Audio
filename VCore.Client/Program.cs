using System;
using System.Text;
using System.Threading.Tasks;
using VCore.Client.Core;
using VCore.Common.Models;

namespace VCore.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Nhóm 21 - V-CORE Messenger Client";
            
            SocketClient client = new SocketClient();
            
            // Đăng ký sự kiện khi nhận được tin nhắn từ Server
            client.OnPacketReceived += (packet) =>
            {
                if (packet.Type == PacketType.Message)
                {
                    string msg = Encoding.UTF8.GetString(packet.Payload);
                    Console.WriteLine($"\n[SERVER GỬI]: {msg}");
                }
            };

            try
            {
                Console.WriteLine("Đang kết nối tới Server (localhost:8080)...");
                await client.ConnectAsync("127.0.0.1", 8080);
                Console.WriteLine("Kết nối THÀNH CÔNG!");

                // Thử đăng nhập
                Console.Write("Nhập tên của bạn: ");
                string? username = Console.ReadLine() ?? "User";
                var loginPacket = new Packet(PacketType.Login, Encoding.UTF8.GetBytes(username));
                await client.SendPacketAsync(loginPacket);

                Console.WriteLine("Gõ tin nhắn và nhấn Enter để gửi (Gõ 'exit' để thoát):");
                while (true)
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit") break;

                    var msgPacket = new Packet(PacketType.Message, Encoding.UTF8.GetBytes(input));
                    await client.SendPacketAsync(msgPacket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI: {ex.Message}");
            }
            finally
            {
                client.Disconnect();
                Console.WriteLine("Đã ngắt kết nối.");
            }
        }
    }
}
