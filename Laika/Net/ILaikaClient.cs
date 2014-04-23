using System;
using Laika.Net.Message;

namespace Laika.Net
{
    public interface ILaikaClient : IDisposable, ILaikaNet
    {
        void BlockingStart();
        void NonBlockingStart();
        void SendAsync(IMessage message);

        event DisconnectedSocketHandle DisconnectedSessionEvent;
        event SocketExceptionHandle OccurredException;
        event ReceiveHandle ReceivedMessage;
    }
}
