﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laika.Net.Message;

namespace Laika.Net
{
    public interface ILaikaServer : IDisposable
    {
        void NonblockingStart();
        void BlockingStart();
        Task SendMessage(Session session, IMessage message);
        void SendMessageAsync(Session session, IMessage message);
        void SendMessageAsync(IEnumerable<Session> sessionList, IMessage message);
        void ReleaseBlocking();
        event ReceiveHandle ReceivedMessageFromSession;
        event ErrorHandle OccurredError;
        event ConnectHandle ConnectedSessionEvent;
        event DisconnectedSocketHandle Disconnect;
        event SendCompletedHandle EventCompletedSendData;
    }
}
