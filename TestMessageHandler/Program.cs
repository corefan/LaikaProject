using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.MessageHandler;

namespace TestMessageHandler
{
    public enum MessageType
    { 
        Login = 0,
        Join = 1,
        RequestXYZ = 2,
    }

    public class MethodSet
    {
        [MessageHandler((int)MessageType.Login)]
        public string LoginMethod(int arg1, int arg2)
        {
            return arg1.ToString() + arg2.ToString();
        }

        [MessageHandler((int)MessageType.Join)]
        public string JoinMethod(int arg1, int arg2)
        {
            return arg1.ToString() + arg2.ToString();
        }

        [MessageHandler((int)MessageType.RequestXYZ)]
        public static string RequstMethod(int arg1, int arg2)
        {
            return arg1.ToString() + arg2.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MessageInvokeHandler<int, int, string> handler = new MessageInvokeHandler<int, int, string>();
            handler.RegisterHandler<MethodSet>();
            Task<string> login = handler.InvokeMethodAsync((int)MessageType.Login, 111, 222);
            Task<string> join = handler.InvokeMethodAsync((int)MessageType.Join, 222, 333);
            Task<string> request = handler.InvokeMethodAsync((int)MessageType.RequestXYZ, 333, 444);

            Console.WriteLine(login.Result);
            Console.WriteLine(join.Result);
            Console.WriteLine(request.Result);
        }
    }
}
