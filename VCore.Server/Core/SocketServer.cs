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
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();
        private bool _isRunning;

        public SocketServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            _isRunning = true;
            Console.WriteLine($"[SERVER] V-CORE Server started on port {((IPEndPoint)_listener.LocalEndpoint).Port}");

            while (_isRunning)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER ERROR] {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            string clientId = Guid.NewGuid().ToString();
            Console.WriteLine($"[SERVER] Client connected: {clientId}");

            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] headerBuffer = new byte[8];
                while (_isRunning)
                {
                    try
                    {
                        // Read Header (8 bytes)
                        int bytesRead = await stream.ReadAsync(headerBuffer, 0, 8);
                        if (bytesRead == 0) break; 

                        int totalSize = BitConverter.ToInt32(headerBuffer, 0);
                        int payloadSize = totalSize - 8;

                        // Read Payload
                        byte[] payloadBuffer = new byte[payloadSize];
                        int totalPayloadRead = 0;
                        while (totalPayloadRead < payloadSize)
                        {
                            int read = await stream.ReadAsync(payloadBuffer, totalPayloadRead, payloadSize - totalPayloadRead);
                            if (read == 0) break;
                            totalPayloadRead += read;
                        }

                        Packet packet = PacketTransporter.Deserialize(headerBuffer, payloadBuffer);
                        await ProcessPacketAsync(clientId, packet, stream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CLIENT ERROR] {clientId}: {ex.Message}");
                        break;
                    }
                }
            }

            _clients.TryRemove(clientId, out _);
            Console.WriteLine($"[SERVER] Client disconnected: {clientId}");
        }

        private async Task ProcessPacketAsync(string clientId, Packet packet, NetworkStream stream)
        {
            if (!packet.VerifyChecksum())
            {
                Console.WriteLine($"[SERVER] WARNING: Checksum failed for packet from {clientId}");
                return;
            }

            switch (packet.Type)
            {
                case PacketType.Login:
                    string username = Encoding.UTF8.GetString(packet.Payload);
                    Console.WriteLine($"[SERVER] User logged in: {username} ({clientId})");
                    break;

                case PacketType.Message:
                    string msg = Encoding.UTF8.GetString(packet.Payload);
                    Console.WriteLine($"[SERVER] Chat from {clientId}: {msg}");
                    // Logic to broadcast/route would go here
                    break;

                case PacketType.Heartbeat:
                    // Respond with heartbeat or just log
                    break;
            }
        }
    }
}
