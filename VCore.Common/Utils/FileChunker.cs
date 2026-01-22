using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VCore.Common.Utils
{
    // Lớp hỗ trợ chia nhỏ file để truyền qua mạng không bị treo
    public static class FileChunker
    {
        // Kích thước mỗi khối là 4KB theo yêu cầu đề tài
        private const int ChunkSize = 4096; 

        // Hàm chia file thành các phần nhỏ (dùng Async Enumerable để tiết kiệm RAM)
        public static async IAsyncEnumerable<byte[]> SplitFileAsync(string filePath)
        {
            if (!File.Exists(filePath)) yield break;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[ChunkSize];
                int bytesRead;
                // Đọc từng khối 4KB cho đến khi hết file
                while ((bytesRead = await fs.ReadAsync(buffer, 0, ChunkSize)) > 0)
                {
                    if (bytesRead < ChunkSize)
                    {
                        // Xử lý khối cuối cùng nếu kích thước nhỏ hơn 4KB
                        byte[] finalBuffer = new byte[bytesRead];
                        Array.Copy(buffer, finalBuffer, bytesRead);
                        yield return finalBuffer;
                    }
                    else
                    {
                        yield return buffer;
                    }
                }
            }
        }

        // Hàm ghép các khối nhận được lại thành file hoàn chỉnh
        public static async Task SaveChunkAsync(string savePath, byte[] chunk)
        {
            using (var fs = new FileStream(savePath, FileMode.Append, FileAccess.Write))
            {
                await fs.WriteAsync(chunk, 0, chunk.Length);
            }
        }
    }
}
