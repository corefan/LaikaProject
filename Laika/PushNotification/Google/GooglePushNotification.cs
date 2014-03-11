using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Laika.ExtendBundle;

namespace Laika.PushNotification
{
    /// <summary>
    /// android push notification service
    /// </summary>
    public class GooglePushNotification : IPushNotification
    {
        private string _apiKey;
        private string _gcmUrl;

        GooglePushNotification(string apiKey, string gcmUrl)
        {
            _apiKey = apiKey;
            _gcmUrl = gcmUrl;
        }
        /// <summary>
        /// service factory
        /// </summary>
        /// <param name="apiKey">Api Key</param>
        /// <param name="gcmUrl">GCM URL</param>
        /// <returns>service interface</returns>
        public static IPushNotification CreateService(string apiKey, string gcmUrl = "https://android.googleapis.com/gcm/send")
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(gcmUrl))
                throw new ArgumentNullException();

            return new GooglePushNotification(apiKey, gcmUrl);
        }
        /// <summary>
        /// send push notification method
        /// </summary>
        /// <param name="message">message string</param>
        /// <param name="deviceToken">device token list</param>
        public void SendPushNotification(string message, IEnumerable<string> deviceToken)
        {
            List<string> deviceTokens = deviceToken.ToList();
            if (deviceTokens.IsNullOrEmpty())
                return;

            if (deviceTokens.Count > 1000)
                throw new Exception("Too much device tokens. limit <= 1000");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_gcmUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add(string.Format("Authorization: key={0}", _apiKey));
            request.SendChunked = false;
            request.Proxy = null;

            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.Append("{\"data\":{\"msg\":\"");
            payloadBuilder.Append(message.Replace("\"", "\\\""));
            payloadBuilder.Append("\"},\"registration_ids\":[");

            var query = from id in deviceTokens select string.Format("\"{0}\"", id);
            string list = string.Join(",", query.ToList());

            payloadBuilder.Append(list);
            payloadBuilder.Append("]}");

            string postDataString = payloadBuilder.ToString();

            using (Stream writeStream = request.GetRequestStream())
            {
                byte[] postData = Encoding.UTF8.GetBytes(postDataString);
                writeStream.Write(postData, 0, postData.Length);
                writeStream.Close();
            }

            WebResponse response = request.GetResponse();
            string result = null;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            //response.Dispose();
        }
    }
}
