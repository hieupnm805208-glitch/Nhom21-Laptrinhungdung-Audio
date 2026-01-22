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

        public event Action<Packet> OnPacketReceived;

        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();
            _isConnected = true;
            
            _ = ReceiveLoopAsync();
        }

        public async Task SendPacketAsync(Packet packet)
        {
            if (!_isConnected) return;
            byte[] data = PacketTransporter.Serialize(packet);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        private async Task ReceiveLoopAsync()
        {
            byte[] headerBuffer = new byte[8];
            try
            {
                while (_isConnected)
                {
                    // Read Header
                    int bytesRead = await _stream.ReadAsync(headerBuffer, 0, 8);
                    if (bytesRead == 0) break;

                    int totalSize = BitConverter.ToInt32(headerBuffer, 0);
                    int payloadSize = totalSize - 8;

                    // Read Payload
                    byte[] payloadBuffer = new byte[payloadSize];
                    int totalPayloadRead = 0;
                    while (totalPayloadRead < payloadSize)
                    {
                        int read = await _stream.ReadAsync(payloadBuffer, totalPayloadRead, payloadSize - payloadPayloadRead);
                        if (read == 0) break;
                        totalPayloadRead += read;
                    }

                    Packet packet = PacketTransporter.Deserialize(headerBuffer, payloadBuffer);
                    OnPacketReceived?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Connection lost: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
            }
        }

        public void Disconnect()
        {
            _isConnected = false;
            _stream?.Close();
            _client?.Close();
        }
    }
}
