using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Net;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;
using System.Net.Sockets;
namespace TestNet
{
    class Header : IHeader
    {
        public int GetHeaderSize()
        {
            return sizeof(int);
        }

        public int ContentsSize { get; set; }
        public int BytesTransferred { get; set; }
        public byte[] HeaderRawData { get; set; }
    }
    class Body : IBody
    {
        public int BytesTransferred { get; set; }
        public byte[] BodyRawData { get; set; }
    }

    class Message : IMessage
    {
        public IHeader Header { get; set; }
        public IBody Body { get; set; }
        public Session Session { get; set; }
        public void SetMessage(byte[] bodyRawData)
        {
            Header = new Header();
            Body = new Body();
            Header.HeaderRawData = BitConverter.GetBytes(bodyRawData.Length);
            Body.BodyRawData = bodyRawData;
        }
    }

    class Program
    {
        static ILaikaServer server = null;
        static ILaikaClient client = null;

        static void Main(string[] args)
        {
            SetTcpServer();
            SetTcpClient();
            Console.ReadKey();
            client.Dispose();
            server.Dispose();
        }

        private static void SetTcpClient()
        {
            client = new LaikaTcpClient<Message, Header, Body>("127.0.0.1", 10000);
            client.DisconnectedSessionEvent += client_DisconnectedSessionEvent;
            client.OccuredException += client_OccuredException;
            client.ReceivedMessage += client_ReceivedMessage;
            client.NonBlockingStart();
            Message m = new Message();
            m.SetMessage(Encoding.UTF8.GetBytes("Hello!"));
            client.SendAsync(m);
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1000);
                client.Dispose();
            });
        }

        static void client_ReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            string result = Encoding.UTF8.GetString(e.Message.Body.BodyRawData);
            Console.WriteLine(result);
        }

        static void client_OccuredException(object sender, ExceptionFromSessionEventArgs e)
        {
            Console.WriteLine("Client Occured Exception {0}", e.Exception.ToString());
        }

        static void client_DisconnectedSessionEvent(object sender, DisconnectSocketEventArgs e)
        {
            Console.WriteLine("socket disconnected. {0}", e.SessionHandle.Handle.Handle.ToInt32());
        }
                        
        private static void SetTcpServer()
        {
            server = new LaikaTcpServer<Message, Header, Body>(10000);
            server.ConnectedSessionEvent += server_ConnectedSocket;
            server.OccuredError += server_OccuredError;
            server.ReceivedMessageFromSession += server_ReceivedMessageFromSession;
            server.NonblockingStart();
        }

        static void server_ReceivedMessageFromSession(object sender, ReceivedMessageEventArgs e)
        {
            Console.WriteLine("Received : {0}", Encoding.UTF8.GetString(e.Message.Body.BodyRawData));
            Message m = new Message();
            string result = "I got it.";
            m.SetMessage(Encoding.UTF8.GetBytes(result));
            server.SendMessage(e.Message.Session, m);
        }

        static void server_ConnectedSocket(object sender, ConnectedSessionEventArgs e)
        {
            Console.WriteLine("Connected : {0}", e.SessionHandle.Handle.Handle.ToInt32());
        }
        
        static void server_OccuredError(object sender, ExceptionEventArgs e)
        {
            Console.WriteLine("Ocurred Error : {0}", e.Exception.ToString());
        }
    }
}
