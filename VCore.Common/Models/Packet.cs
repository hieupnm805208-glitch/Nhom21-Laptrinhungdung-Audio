using System;

namespace VCore.Common.Models
{
    // Định nghĩa các loại gói tin trong hệ thống
    public enum PacketType : ushort
    {
        Login = 1,      // Đăng nhập
        Message = 2,    // Tin nhắn văn bản
        FileInit = 3,   // Khởi tạo gửi file
        FileChunk = 4,  // Các khối dữ liệu của file
        Heartbeat = 5,  // Kiểm tra kết nối
        Disconnect = 6, // Ngắt kết nối
        Error = 99      // Lỗi hệ thống
    }

    public class Packet
    {
        public PacketType Type { get; set; }     // Loại gói tin
        public ushort Checksum { get; set; }    // Mã kiểm tra toàn vẹn
        public byte[] Payload { get; set; }     // Nội dung dữ liệu

        public Packet()
        {
            Payload = Array.Empty<byte>();
        }

        // Constructor để tạo nhanh một gói tin mới
        public Packet(PacketType type, byte[] payload)
        {
            Type = type;
            Payload = payload ?? Array.Empty<byte>();
            Checksum = CalculateChecksum(Payload); // Tự động tính checksum khi tạo
        }

        // Hàm tính Checksum đơn giản bằng cách cộng dồn các byte
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

        // Kiểm tra xem dữ liệu nhận được có khớp với checksum không
        public bool VerifyChecksum()
        {
            return Checksum == CalculateChecksum(Payload);
        }
    }
}
