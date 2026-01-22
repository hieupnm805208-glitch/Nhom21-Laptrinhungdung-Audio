using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VCore.Common.Models;

namespace VCore.Common.Utils
{
    public static class FileChunker
    {
        private const int ChunkSize = 4096; // 4KB chunks as per requirement

        public static async IAsyncEnumerable<byte[]> SplitFileAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[ChunkSize];
                int bytesRead;
                while ((bytesRead = await fs.ReadAsync(buffer, 0, ChunkSize)) > 0)
                {
                    if (bytesRead < ChunkSize)
                    {
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

        public static async Task SaveChunkAsync(string savePath, byte[] chunk)
        {
            using (var fs = new FileStream(savePath, FileMode.Append, FileAccess.Write))
            {
                await fs.WriteAsync(chunk, 0, chunk.Length);
            }
        }
    }
}
