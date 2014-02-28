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
using Laika.Database;
using Laika.Database.Sharding;
using Laika.Database.MySql;
using MySql.Data.MySqlClient;

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

    class sharding : Laika.Database.Sharding.DatabaseShardingBase<int, string>
    {
        /// <summary>
        /// 현재 DB key가 shardkey와 같을 경우 아래와 같이 재정의 합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override IDatabase FindDB(int key)
        {
            string shardKey = key.ToString();
            if (AllDataBase.ContainsKey(shardKey) == false)
                return null;

            return AllDataBase[shardKey];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Laika.ThreadPoolManager.AppDomainThreadPoolManager.SetMinThreadPool(100, 100);

            DBTest();
            //TestMessageHandler();
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

        private static void DBTest()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.UserID = "root";
            sb.Password = "alsdl12#$";
            sb.Database = "test";
            sb.Server = "localhost";
            sb.Pooling = true;
            sb.MinimumPoolSize = 10;
            sb.MaximumPoolSize = 20;
            IDatabase db = CreateDbFactory.CreateMySqlDatabase(sb);
            MySqlDbJob job = MySqlDbJob.CreateMySqlTransactionJob(x => 
            {
                using (MySqlCommand cmd = x.CreateDbCommand("INSERT INTO user SET uid=1, score = 100, last_play=NOW();"))
                {
                    cmd.ExecuteNonQuery();
                }

                int score = 0;
                using (MySqlCommand cmd = x.CreateDbCommand("SELECT score FROM user WHERE uid=1;"))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        score = reader.GetInt32(0);
                    }
                }

                using (MySqlCommand cmd = x.CreateDbCommand("UPDATE user SET score = score + 1 WHERE uid=1;"))
                {
                    cmd.ExecuteNonQuery();
                }
            }, ex => { Console.WriteLine(ex.ToString()); });

            db.AsyncDoJob(job).Wait();
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
