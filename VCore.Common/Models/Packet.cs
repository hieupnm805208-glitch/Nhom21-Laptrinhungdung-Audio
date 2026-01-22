using System;

namespace VCore.Common.Models
{
    public enum PacketType : ushort
    {
        Login = 1,
        Message = 2,
        FileInit = 3,
        FileChunk = 4,
        Heartbeat = 5,
        Disconnect = 6,
        Error = 99
    }

    public class Packet
    {
        public PacketType Type { get; set; }
        public ushort Checksum { get; set; }
        public byte[] Payload { get; set; }

        public Packet()
        {
            Payload = Array.Empty<byte>();
        }

        public Packet(PacketType type, byte[] payload)
        {
            Type = type;
            Payload = payload ?? Array.Empty<byte>();
            Checksum = CalculateChecksum(Payload);
        }

        public static ushort CalculateChecksum(byte[] data)
        {
            if (data == null) return 0;
            ushort sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }
            return sum;
        }

        public bool VerifyChecksum()
        {
            return Checksum == CalculateChecksum(Payload);
        }
    }
}
