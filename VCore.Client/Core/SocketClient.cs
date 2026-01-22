using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using VCore.Common.Models;
using VCore.Common.Protocol;

namespace VCore.Client.Core
{
    public class SocketClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;

        // Sự kiện thông báo khi có gói tin mới từ Server gửi về
        public event Action<Packet> OnPacketReceived;

        // Kết nối tới Server
        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();
            _isConnected = true;
            
            // Bắt đầu vòng lặp nhận dữ liệu chạy ngầm
            _ = ReceiveLoopAsync();
        }

        // Gửi gói tin lên Server
        public async Task SendPacketAsync(Packet packet)
        {
            if (!_isConnected) return;
            // Sử dụng Helper để biến đối tượng thành mảng byte
            byte[] data = PacketHelper.Serialize(packet);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        // Vòng lặp nhận dữ liệu (luôn lắng nghe Server)
        private async Task ReceiveLoopAsync()
        {
            byte[] headerBuffer = new byte[8];
            try
            {
                while (_isConnected)
                {
                    // Đọc tiêu đề (Header)
                    int bytesRead = await _stream.ReadAsync(headerBuffer, 0, 8);
                    if (bytesRead == 0) break;

                    int totalSize = BitConverter.ToInt32(headerBuffer, 0);
                    int payloadSize = totalSize - 8;

                    // Đọc nội dung (Payload)
                    byte[] payloadBuffer = new byte[payloadSize];
                    int totalPayloadRead = 0;
                    while (totalPayloadRead < payloadSize)
                    {
                        int read = await _stream.ReadAsync(payloadBuffer, totalPayloadRead, payloadSize - totalPayloadRead);
                        if (read == 0) break;
                        totalPayloadRead += read;
                    }

                    // Chuyển đổi byte về dạng đối tượng để xử lý ở giao diện (UI)
                    Packet packet = PacketHelper.Deserialize(headerBuffer, payloadBuffer);
                    OnPacketReceived?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Mất kết nối tới Server: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
            }
        }

        // Chủ động ngắt kết nối
        public void Disconnect()
        {
            _isConnected = false;
            _stream?.Close();
            _client?.Close();
        }
    }
}
