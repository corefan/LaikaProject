using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;
namespace ConsoleApplication1
{
    public class Message : IMessage
    {
        public Socket socket { get; set; }
        public List<Socket> sockets { get; set; }
        public IHeader Header { get; set; }
        public IBody Body { get; set; }

        /// <summary>
        /// 메시지 세팅.
        /// </summary>
        /// <param name="receiver">받는 소켓 LaikaTcpServer에서 LaikaTcpClient 보낼 경우 명시되어야 한다.</param>
        /// <param name="bodyRawData">body bytes data</param>
        public void SetMessage(Socket receiver, byte[] bodyRawData)
        {
            socket = receiver;
            SetMessage(bodyRawData);

        }
        /// <summary>
        /// 메시지 세팅. LaikaTcpClient의 경우 LaikaTcpServer를 알고 있기 때문에 socket 명시가 필요하지 않음.
        /// </summary>
        /// <param name="bodyRawData">body bytes data</param>
        public void SetMessage(byte[] bodyRawData)
        {
            Header = new Header();
            Body = new Body();
            Body.BodyRawData = bodyRawData;
            Header.HeaderRawData = BitConverter.GetBytes(Body.BodyRawData.Length);
        }

        /// <summary>
        /// 메시지 세팅
        /// </summary>
        /// <param name="receivers">받는 소켓이 여러 곳일 경우</param>
        /// <param name="bodyRawData">body bytes data</param>
        public void SetMessage(List<Socket> receivers, byte[] bodyRawData)
        {
            sockets = receivers;
            SetMessage(bodyRawData);
        }
    }
}
