using System;
using System.IO;
using VCore.Common.Models;

namespace VCore.Common.Protocol
{
    public static class PacketTransporter
    {
        private const int HeaderSize = 8; // 4 (Size) + 2 (Type) + 2 (Checksum)

        public static byte[] Serialize(Packet packet)
        {
            int totalSize = HeaderSize + packet.Payload.Length;
            byte[] buffer = new byte[totalSize];

            using (var ms = new MemoryStream(buffer))
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(totalSize);
                writer.Write((ushort)packet.Type);
                writer.Write(packet.Checksum);
                writer.Write(packet.Payload);
            }

            return buffer;
        }

        public static Packet Deserialize(byte[] headerData, byte[] payloadData)
        {
            if (headerData.Length < HeaderSize)
                throw new ArgumentException("Invalid header size");

            using (var ms = new MemoryStream(headerData))
            using (var reader = new BinaryReader(ms))
            {
                int totalSize = reader.ReadInt32(); // Read total size (redundant here but stays true to protocol)
                PacketType type = (PacketType)reader.ReadUInt16();
                ushort checksum = reader.ReadUInt16();

                return new Packet
                {
                    Type = type,
                    Checksum = checksum,
                    Payload = payloadData
                };
            }
        }
    }
}
