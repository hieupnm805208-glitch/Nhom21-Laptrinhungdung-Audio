using System;
using System.IO;
using VCore.Common.Models;

namespace VCore.Common.Protocol
{
    // Lớp hỗ trợ đóng gói và giải nén gói tin (Serialization/Deserialization)
    public static class PacketHelper
    {
        // Kích thước header cố định là 8 byte
        private const int HeaderSize = 8; 

        // Chuyển đối tượng Packet thành mảng Byte để gửi qua Socket
        public static byte[] Serialize(Packet packet)
        {
            int totalSize = HeaderSize + packet.Payload.Length;
            byte[] buffer = new byte[totalSize];

            using (var ms = new MemoryStream(buffer))
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(totalSize);               // 4 bytes: Tổng kích thước
                writer.Write((ushort)packet.Type);     // 2 bytes: Loại gói tin
                writer.Write(packet.Checksum);         // 2 bytes: Mã checksum
                writer.Write(packet.Payload);          // Các byte còn lại: Dữ liệu thực tế
            }

            return buffer;
        }

        // Chuyển mảng Byte nhận được từ Socket thành đối tượng Packet
        public static Packet Deserialize(byte[] headerData, byte[] payloadData)
        {
            if (headerData.Length < HeaderSize)
                throw new ArgumentException("Header không hợp lệ (nhỏ hơn 8 byte)");

            using (var ms = new MemoryStream(headerData))
            using (var reader = new BinaryReader(ms))
            {
                int totalSize = reader.ReadInt32(); 
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
