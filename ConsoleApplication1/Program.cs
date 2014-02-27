using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Laika.Log;
using Laika.Net;
using Laika.Crypto;
using Laika.PushNotification;
using Laika.Crash;
using Laika.MessageHandler;

namespace ConsoleApplication1
{
    public enum MessageType
    {
        Login = 0,
        Join,
    }

    class Logic
    {
        [MessageHandler((int)MessageType.Login)]
        public string Login(int arg)
        {
            return arg.ToString();
        }

        [MessageHandler((int)MessageType.Join)]
        public string Join(int arg)
        {
            return arg.ToString();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            TestMessageHandler();
            //TripleDesTest();
            //FileLogTest();
            //CrashTest();
            //PushTest();
            //MD5Test();
            //ServerTest();
            //ClientTest();
            Console.WriteLine("End");
            Console.ReadKey();
        }

        private static void TestMessageHandler()
        {
            MessageInvokeHandler<int, string> handler = new MessageInvokeHandler<int, string>();

            handler.RegisterHandler<Logic>();
            handler.InvokeMethod(0, 0);
            Task<string> t = handler.InvokeMethodAsync(1, 1);

            t.ContinueWith(_t => { Console.WriteLine(_t.Result); });
        }

        private static void TripleDesTest()
        {
            LaikaTripleDES des = new LaikaTripleDES(Encoding.UTF8.GetBytes("1234567890123456"));
            byte[] r = des.Encrypt(Encoding.UTF8.GetBytes("1234567890123456"));
            byte[] r1 = des.Decrypt(r);
            string s = Encoding.UTF8.GetString(r1);
        }

        private static void CrashTest()
        {
            CrashHandler c = new CrashHandler();
            c.RegisterCrashLogWriter();
            throw new Exception("2");
        }

        private static void PushTest()
        {
            
        }

        private static void MD5Test()
        {
            byte[] result = LaikaMD5.GetMd5Hash(null);
            byte[] result1 = LaikaMD5.GetMd5Hash(Encoding.UTF8.GetBytes("hello"));
            string r1 = LaikaMD5.GetStringFromMd5hashBytes(result1);
            string r2 = LaikaMD5.GetStringFromMd5hashBytes(result1, true);
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
