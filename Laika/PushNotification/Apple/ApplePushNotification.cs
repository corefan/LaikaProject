using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using Laika.ExtendBundle;

namespace Laika.PushNotification
{
    /// <summary>
    /// Apple push notification service
    /// </summary>
    public class ApplePushNotification : IPushNotification
    {
        private APNSInfo _apnsInfo;
        ApplePushNotification(APNSInfo info)
        {
            _apnsInfo = info;
        }
        /// <summary>
        /// push notification factory
        /// </summary>
        /// <param name="serverUrl">APNS URL</param>
        /// <param name="serverPort">APNS PORT</param>
        /// <param name="certPath">CERT Path</param>
        /// <param name="certPassword">CERT Password</param>
        /// <param name="production">production : boolean</param>
        /// <returns>Notification interface</returns>
        public static ApplePushNotification CreateService(string serverUrl, int serverPort, string certPath, string certPassword, bool production)
        {
            APNSInfo info = new APNSInfo(serverUrl, serverPort, certPath, certPassword, production);

            return new ApplePushNotification(info);
        }
        /// <summary>
        /// Send push notification method
        /// </summary>
        /// <param name="message">Message string</param>
        /// <param name="deviceToken">Device token list</param>
        public void SendPushNotification(string message, IEnumerable<string> deviceToken)
        {
            StringBuilder payLoad = GetPayload(message);
            SendMessage(deviceToken.ToList(), payLoad.ToString());
        }

        private void SendMessage(List<string> tokens, string message)
        {
            if (tokens.IsNullOrEmpty())
                return;

            X509Certificate2 clientCert = new X509Certificate2(_apnsInfo.CertPath, _apnsInfo.CertPassword);
            X509Certificate2Collection certCollection = new X509Certificate2Collection(clientCert);

            TcpClient client = new TcpClient(_apnsInfo.ServerUrl, _apnsInfo.Port);

            using (SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
            {
                sslStream.AuthenticateAsClient(_apnsInfo.ServerUrl, certCollection, SslProtocols.Tls, true);
                byte[] payLoadByteData = Encoding.UTF8.GetBytes(message);

                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    foreach (string token in tokens)
                    {
                        writer.Write((byte)0);  //The command
                        writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
                        writer.Write((byte)32); //The deviceId length (big-endian second byte)
                        writer.Write(HexStringToByteArray(token.ToUpper()));
                        writer.Write((byte)0);
                        writer.Write((byte)payLoadByteData.Length);
                        writer.Write(payLoadByteData);
                    }
                    writer.Flush();
                    byte[] array = memoryStream.ToArray();
                    sslStream.Write(array);
                    sslStream.Flush();
                }
            }
            client.Close();
        }

        private StringBuilder GetPayloadWithBadge(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"aps\":{\"alert\":\"");
            sb.Append(message.Replace("\"", "\\\""));
            sb.Append("\",\"badge\":1,\"sound\":\"default\"}}");
            return sb;
        }

        private StringBuilder GetPayload(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"aps\":{\"alert\":\"");
            sb.Append(message.Replace("\"", "\\\""));
            sb.Append("\",\"sound\":\"default\"}}");
            return sb;
        }

        private bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private byte[] HexStringToByteArray(string source)
        {
            source = source.Replace(" ", "");
            byte[] buffer = new byte[source.Length / 2];
            for (int i = 0; i < source.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(source.Substring(i, 2), 16);
            }
            return buffer;
        }
    }
}
