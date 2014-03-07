using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Laika.Compression.GZip
{
    /// <summary>
    /// GZip 압축, 압축해제 클래스
    /// </summary>
    public class GZip
    {
        /// <summary>
        /// 압축
        /// </summary>
        /// <param name="raw">raw 데이터</param>
        /// <returns>compressed 데이터</returns>
        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }
        /// <summary>
        /// 압축해제
        /// </summary>
        /// <param name="compressed">compressed 데이터</param>
        /// <returns>raw 데이터</returns>
        public static byte[] DeCompress(byte[] compressed)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(compressed), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                    } while (count > 0);

                    return ms.ToArray();
                }
            }
        }
    }
}
