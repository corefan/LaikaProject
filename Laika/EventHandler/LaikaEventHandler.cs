
namespace Laika.Net
{
    public delegate void ReceiveHandle(object sender, ReceivedMessageEventArgs e);
    public delegate void ErrorHandle(object sender, ExceptionEventArgs e);
    public delegate void ConnectHandle(object sender, ConnectedSessionEventArgs e);
    public delegate void DisconnectedSocketHandle(object sender, DisconnectSocketEventArgs e);
    public delegate void SocketExceptionHandle(object sender, ExceptionFromSessionEventArgs e);
    public delegate void ExceptionSessionHandle(object sender, ExceptionFromSessionEventArgs e);
}

namespace Laika.Web
{
    public delegate void ResponseEventHandler(object sender, AsyncResponseEventArgs e);
}

namespace Laika.Event
{
    public delegate void EndEventHandler(object sender, TaskServiceEndEventArgs e);
}