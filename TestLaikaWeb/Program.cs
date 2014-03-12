using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Laika.Web;

namespace TestLaikaWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWebRequest();

            ManualResetEventSlim mre = new ManualResetEventSlim(false);
            mre.Wait();
        }

        private static void TestWebRequest()
        {
            LaikaWeb web = new LaikaWeb("http://www.google.com");
            web.RequestResult += (sender, e) => { Console.WriteLine(e.ResponseData); };

            // sync 
            Console.WriteLine(web.Request(ContentType.text_html));

            // async
            web.RequestAsync(ContentType.text_html);
        }
    }
}
