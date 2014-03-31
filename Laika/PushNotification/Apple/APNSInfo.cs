
namespace Laika.PushNotification
{
    internal class APNSInfo
    {
        internal APNSInfo(string pushServerUrl, int pushServerPort, string pushCertificatePath, string certPassword, bool production)
        {
            this.ServerUrl = pushServerUrl;
            this.Port = pushServerPort;
            this.CertPath = pushCertificatePath;
            this.CertPassword = certPassword;
            this.Production = production;
        }

        internal string ServerUrl { get; private set; }

        internal int Port { get; private set; }

        internal string CertPath { get; private set; }

        internal string CertPassword { get; private set; }

        internal bool Production { get; private set; }
    }
}
