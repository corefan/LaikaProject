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
        /// <summary>
        /// client socket.
        /// 받은 메시지 일 경우 보낸자의 socket
        /// 보내는 메시지 일 경우 받는자의 socket
        /// </summary>
        Socket socket { get; set; }
        /// <summary>
        /// client sockets.
        /// 메시지의 수신자가 여러명일 경우 List 사용
        /// </summary>
        List<Socket> sockets { get; set; }
        /// <summary>
        /// 메시지 세팅
        /// </summary>
        /// <param name="receivers">받는 소켓이 여러 곳일 경우</param>
        /// <param name="bodyRawData">body bytes data</param>
        void SetMessage(List<Socket> receivers, byte[] bodyRawData);
        /// <summary>
        /// 메시지 세팅.
        /// </summary>
        /// <param name="receiver">받는 소켓 LaikaTcpServer에서 LaikaTcpClient 보낼 경우 명시되어야 한다.</param>
        /// <param name="bodyRawData">body bytes data</param>
        void SetMessage(Socket receiver, byte[] bodyRawData);
        /// <summary>
        /// 메시지 세팅. LaikaTcpClient의 경우 LaikaTcpServer를 알고 있기 때문에 socket 명시가 필요하지 않음.
        /// </summary>
        /// <param name="bodyRawData">body bytes data</param>
        void SetMessage(byte[] bodyRawData);
    }
}
