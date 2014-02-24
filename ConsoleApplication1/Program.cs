using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Laika.Log;
using Laika.Net;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //FileLogTest();

            ServerTest();
            ClientTest();

            Console.ReadKey();
        }

        private static void ClientTest()
        {
            string echo = "Hello.";
            client = new LaikaTcpClient<Message, Header, Body>("10.10.30.137", 10000);
            client.ReceivedMessage += client_ReceivedMessage;
            client.Poll();
            int i = 0;
            while (i < 5)
            {
                string sendData = string.Format("{0}_{1}", echo, i);
                Message m = new Message();
                m.SetMessage(Encoding.UTF8.GetBytes(sendData));
                Console.WriteLine("Client sending.. : {0}", sendData);
                client.SendAsync(m);
                i++;
            }
        }

        static void client_ReceivedMessage(Laika.Net.Message.IMessage message)
        {
            string str = Encoding.UTF8.GetString(message.Body.BodyRawData);
            Console.WriteLine("Client received : {0}", str);
        }

        private static void ServerTest()
        {
            server = new LaikaTcpServer<Message, Header, Body>(10000);
            server.ReceivedMessageFromClient += server_ReceivedMessageFromClient;
            server.ConnectedSocket += server_ConnectedSocket;
            server.Poll();
        }

        static void server_ConnectedSocket(System.Net.Sockets.Socket socket)
        {
            Console.WriteLine("Socket Conntected : {0}", socket.Handle.ToInt32());
        }

        private static void server_ReceivedMessageFromClient(System.Net.Sockets.Socket socket, Laika.Net.Message.IMessage message)
        {
            Console.WriteLine("Server received : {0}", Encoding.UTF8.GetString(message.Body.BodyRawData));
            Console.WriteLine("Server Sending : {0}", Encoding.UTF8.GetString(message.Body.BodyRawData));
            Message m = new Message();
            m.SetMessage(socket, message.Body.BodyRawData);
            
            server.SendMessage(m);
        }

        private static LaikaTcpClient<Message, Header, Body> client;
        private static LaikaTcpServer<Message, Header, Body> server;

        private static void FileLogTest()
        {
            FileLogParameter param = new FileLogParameter();
            param.FileName = "SPEED";
            param.Type = PartitionType.TIME;
            param.Debug = true;
            int i = 0;
            IFileLog log = FileLogFactory.CreateFileLog(param);

            while (i < 500000)
            {
                log.DEBUG_LOG("Debug");
                log.ERROR_LOG("Error");
                log.FATAL_LOG("fatal");
                log.INFO_LOG("info");
                log.WARNING_LOG("warn");
                i++;
            }
            log.Dispose();
        }
    }
}
