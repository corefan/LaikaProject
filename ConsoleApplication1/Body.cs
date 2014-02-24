using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.Net.Body;

namespace ConsoleApplication1
{
    public class Body : IBody
    {
        /// <summary>
        /// Transferred bytes
        /// </summary>
        public int BytesTransferred { get; set; }
        /// <summary>
        /// Body raw data
        /// </summary>
        public byte[] BodyRawData { get; set; }
    }
}
