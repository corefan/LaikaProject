using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Compression.GZip;

namespace TestCompress
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] raw = Encoding.UTF8.GetBytes("Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!Hello, World!");
            byte[] compressed = GZip.Compress(raw);
            byte[] decompressed = GZip.DeCompress(compressed);
            string recovery = Encoding.UTF8.GetString(decompressed);
        }
    }
}
