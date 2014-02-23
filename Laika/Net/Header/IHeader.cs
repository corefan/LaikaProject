using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Net.Body;

namespace Laika.Net.Header
{
    /// <summary>
    /// Header Interface
    /// </summary>
    public interface IHeader
    {
        /// <summary>
        /// Header의 총 사이즈 입니다. 고정 사이즈여야 합니다.
        /// </summary>
        int GetHeaderSize();
        /// <summary>
        /// ContentsSize는 Header Raw Data의 최상위 4Bytes입니다.
        /// </summary>
        int ContentsSize { get; set; }
        /// <summary>
        /// 전송된 사이즈
        /// </summary>
        int BytesTransferred { get; set; }
        /// <summary>
        /// Header byte data
        /// </summary>
        byte[] HeaderRawData { get; set; }
    }
}
