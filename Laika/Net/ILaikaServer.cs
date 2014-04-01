using System;
using System.Collections.Generic;
using Laika.Net.Message;

namespace Laika.Net
{
    public interface ILaikaServer : IDisposable
    {
        void NonblockingStart();
        void BlockingStart();
        void SendMessage(Session session, IMessage message);
        void SendMessage(IEnumerable<Session> sessionList, IMessage message);
        event ReceiveHandle ReceivedMessageFromSession;
        event ErrorHandle OccuredError;
        event ConnectHandle ConnectedSessionEvent;
        event DisconnectedSocketHandle Disconnect;
    }
}
