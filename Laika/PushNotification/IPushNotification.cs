using System.Collections.Generic;

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
