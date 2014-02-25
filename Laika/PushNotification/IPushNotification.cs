using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.PushNotification
{
    /// <summary>
    /// push notification service interface
    /// </summary>
    public interface IPushNotification
    {
        /// <summary>
        /// message send method
        /// </summary>
        /// <param name="message">message string</param>
        /// <param name="deviceToken">device list</param>
        void SendPushNotification(string message, IEnumerable<string> deviceToken);
    }
}
