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
        //{
        //    get
        //    {
        //        return _messageHeader;
        //    }
        //}

        //private Header _messageHeader;
        public IBody Body { get; set; }
        public Socket socket { get; set; }
        public List<Socket> sockets { get; set; }
        
        public void SetMessage(List<Socket> receivers, byte[] bodyRawData)
        {
            sockets = receivers;
            SetData(bodyRawData);
        }

        private void SetData(byte[] bodyRawData)
        {
            Body = new Body();
            Body.BodyRawData = bodyRawData;
            Header = new Header();
            Header.HeaderRawData = BitConverter.GetBytes(bodyRawData.Length);
        }

        public void SetMessage(Socket receiver, byte[] bodyRawData)
        {
            socket = receiver;
            SetData(bodyRawData);
        }

        public void SetMessage(byte[] bodyRawData)
        {
            SetData(bodyRawData);
        }
    }

    class Program
    {
        static LaikaTcpServer<Message, Header, Body> server = null;
        static LaikaTcpClient<Message, Header, Body> client = null;

        static void Main(string[] args)
        {
            SetTcpServer();
            SetTcpClient();
            Console.ReadKey();
        }

        private static void SetTcpClient()
        {
            client = new LaikaTcpClient<Message, Header, Body>("127.0.0.1", 10000);
            client.DisconnectedSocket += client_DisconnectedSocket;
            client.OccuredException += client_OccuredException;
            client.ReceivedMessage += client_ReceivedMessage;
            client.Poll();
            Message m = new Message();
            m.SetMessage(Encoding.UTF8.GetBytes("Hello!"));
            client.SendAsync(m);
            Task.Factory.StartNew(() => 
            {
                System.Threading.Thread.Sleep(1000);
                client.Dispose();
            });
        }

        static void client_ReceivedMessage(IMessage message)
        {
            string result = Encoding.UTF8.GetString(message.Body.BodyRawData);
            Console.WriteLine(result);
        }

        static void client_OccuredException(Exception ex)
        {
            Console.WriteLine("Client Occured Exception {0}", ex.ToString());
        }

        static void client_DisconnectedSocket(Socket socket)
        {
            Console.WriteLine("socket disconnected. {0}", socket.Handle.ToInt32());
        }
        
        private static void SetTcpServer()
        {
            server = new LaikaTcpServer<Message, Header, Body>(10000);
            server.ConnectedSocket += server_ConnectedSocket;
            server.OccuredError += server_OccuredError;
            server.ReceivedMessageFromClient += server_ReceivedMessageFromClient;
            server.Poll();
        }

        static void server_ReceivedMessageFromClient(Socket socket, IMessage message)
        {
            Console.WriteLine("Received : {0}", Encoding.UTF8.GetString(message.Body.BodyRawData));
            Message m = new Message();
            string result = "I got it.";
            m.SetMessage(socket, Encoding.UTF8.GetBytes(result));
            server.SendMessage(m);
        }

        static void server_OccuredError(Exception ex)
        {
            Console.WriteLine("Ocurred Error : {0}", ex.ToString());
        }

        static void server_ConnectedSocket(Socket socket)
        {
            Console.WriteLine("Connected : {0}", socket.Handle.ToInt32());
        }
    }
}
