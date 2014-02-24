using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.Net.Header;

namespace ConsoleApplication1
{
    public class Header : IHeader
    {
        /// <summary>
        /// Header의 총 사이즈 입니다. 고정 사이즈여야 합니다.
        /// </summary>
        public int GetHeaderSize()
        {
            return HeaderSize;
        }
        /// <summary>
        /// ContentsSize는 Header Raw Data의 최상위 4Bytes입니다.
        /// </summary>
        public int ContentsSize { get; set; }
        /// <summary>
        /// 전송된 사이즈
        /// </summary>
        public int BytesTransferred { get; set; }
        /// <summary>
        /// Header byte data
        /// </summary>
        public byte[] HeaderRawData { get; set; }

        private const int HeaderSize = sizeof(int);
    }
}
