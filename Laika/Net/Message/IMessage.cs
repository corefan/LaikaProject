using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net.Message
{
    /// <summary>
    /// 클라이언트와 서버간 송수신 하는 메시지 단위
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 헤더
        /// </summary>
        IHeader Header { get; set; }
        /// <summary>
        /// 바디
        /// </summary>
        IBody Body { get; set; }

        Session Session { get; set; }
    }
}
