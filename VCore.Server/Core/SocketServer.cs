using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VCore.Common.Models;
using VCore.Common.Protocol;

namespace VCore.Server.Core
{
    public class SocketServer
    {
        private readonly TcpListener _listener;
        // Danh sách quản lý các Client đang kết nối (Thread-safe)
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();
        private bool _isRunning;

        public SocketServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        // Hàm bắt đầu chạy Server
        public async Task StartAsync()
        {
            _listener.Start();
            _isRunning = true;
            AuditLogger.Log($"Server đã khởi động tại cổng {((IPEndPoint)_listener.LocalEndpoint).Port}");

            while (_isRunning)
            {
                try
                {
                    // Chấp nhận kết nối mới từ Client
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    // Xử lý mỗi Client trên một luồng riêng (Bất đồng bộ)
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    AuditLogger.Error("Lỗi khi chấp nhận kết nối", ex);
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            string clientId = Guid.NewGuid().ToString();
            AuditLogger.Log($"Client mới kết nối. Mã ID: {clientId}");

            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] headerBuffer = new byte[8]; // Header luôn có 8 byte theo thiết kế
                while (_isRunning)
                {
                    try
                    {
                        // Bước 1: Đọc Header để biết kích thước gói tin
                        int bytesRead = await stream.ReadAsync(headerBuffer, 0, 8);
                        if (bytesRead == 0) break; // Client ngắt kết nối chủ động

                        int totalSize = BitConverter.ToInt32(headerBuffer, 0);
                        int payloadSize = totalSize - 8;

                        // Bước 2: Đọc nốt phần dữ liệu (Payload) dựa trên kích thước đã biết
                        byte[] payloadBuffer = new byte[payloadSize];
                        int totalPayloadRead = 0;
                        while (totalPayloadRead < payloadSize)
                        {
                            int read = await stream.ReadAsync(payloadBuffer, totalPayloadRead, payloadSize - totalPayloadRead);
                            if (read == 0) break;
                            totalPayloadRead += read;
                        }

                        // Giải nén gói tin bằng Helper
                        Packet packet = PacketHelper.Deserialize(headerBuffer, payloadBuffer);
                        await ProcessPacketAsync(clientId, packet, stream);
                    }
                    catch (Exception ex)
                    {
                        AuditLogger.Error($"Lỗi khi giao tiếp với Client {clientId}", ex);
                        break;
                    }
                }
            }

            _clients.TryRemove(clientId, out _);
            AuditLogger.Log($"Client đã rời khỏi hệ thống: {clientId}");
        }

        // Hàm xử lý logic cụ thể cho từng loại gói tin
        private async Task ProcessPacketAsync(string clientId, Packet packet, NetworkStream stream)
        {
            // Kiểm tra tính toàn vẹn ngay khi nhận
            if (!packet.VerifyChecksum())
            {
                AuditLogger.Log($"CẢNH BÁO: Checksum không khớp từ {clientId}", "WARNING");
                return;
            }

            switch (packet.Type)
            {
                case PacketType.Login:
                    string username = Encoding.UTF8.GetString(packet.Payload);
                    AuditLogger.Log($"Người dùng đăng nhập thành công: {username} ({clientId})");
                    break;

                case PacketType.Message:
                    string msg = Encoding.UTF8.GetString(packet.Payload);
                    AuditLogger.Log($"Tin nhắn mới từ {clientId}: {msg}");
                    
                    // Gửi ngược lại cho Client để xác nhận (Echo)
                    byte[] responseData = Encoding.UTF8.GetBytes($"Server đã nhận: {msg}");
                    Packet response = new Packet(PacketType.Message, responseData);
                    byte[] serializedResponse = PacketHelper.Serialize(response);
                    await stream.WriteAsync(serializedResponse, 0, serializedResponse.Length);
                    break;

                case PacketType.Heartbeat:
                    // Server chỉ nhận, có thể phản hồi lại nếu cần
                    break;
            }
        }
    }
}
