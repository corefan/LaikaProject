using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.PushNotification;

namespace TestPushNotice
{
    class Program
    {
        static void Main(string[] args)
        {
            IPushNotification apple = ApplePushNotification.CreateService("url", 1111, "certpath", "certpassword", false);
            IPushNotification google = GooglePushNotification.CreateService("api_key");
            List<string> deviceToken = new List<string>();
            deviceToken.Add("123456789");

            apple.SendPushNotification("go apple", deviceToken);
            google.SendPushNotification("go google", deviceToken);
        }
    }
}
