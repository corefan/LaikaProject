using System.Net.Sockets;
namespace Laika.Net
{
    public class Session
    {
        public Socket Handle { get; internal set; }
    }
}
