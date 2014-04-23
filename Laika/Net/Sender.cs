using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    internal class Sender
    {
        private void SendMessageToSocketAsync(Session session, byte[] sendData)
        {
            session.SendQueue.Enqueue(sendData);

            if (session.NeedSendingProcedure() == false)
                return;

            Task.Factory.StartNew(() => 
            {
                SendToSession(session);
            });
        }

        private void SendToSession(Session session)
        {
            byte[] data;
            if (session.SendQueue.TryDequeue(out data) == false)
            {
                session.EndSend();
                return;
            }

            session.Context.SendData = data;

            session.SendEventArgs.SetBuffer(session.Context.SendData, 0, session.Context.SendData.Length);
            if (session.SetArgsCompleted == false)
            {
                session.SendEventArgs.UserToken = session;
                session.SendEventArgs.Completed += SendCompleted;
                session.SetArgsCompleted = true;
            }

            if (session.Handle.SendAsync(session.SendEventArgs) == false)
                SendCompleted(session.Handle, session.SendEventArgs);
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            Session session = e.UserToken as Session;
            try
            {
                if (session.Handle == null)
                    throw new ArgumentNullException();

                int bytesTransferred = e.BytesTransferred;
                session.Context.BytesTransferred += bytesTransferred;

                if (bytesTransferred <= 0)
                {
                    if (DisconnectedSession != null)
                        DisconnectedSession(this, new DisconnectSocketEventArgs(session));

                    CleanArgument(e, session.Context);
                    return;
                }
                else if (session.Context.BytesTransferred < session.Context.SendData.Length)
                {
                    e.UserToken = session;
                    e.SetBuffer(session.Context.SendData, session.Context.BytesTransferred, session.Context.SendData.Length - session.Context.BytesTransferred);

                    if (session.Handle.SendAsync(e) == false)
                        SendCompleted(session.Handle, e);
                }
                else if (session.Context.BytesTransferred == session.Context.SendData.Length)
                {
                    if (EventCompletedSendData != null)
                        EventCompletedSendData(this, new SendMessageEventArgs(session));
                    CleanArgument(e, session.Context);
                    SendToSession(session);
                }
                else
                {
                    CleanArgument(e, session.Context);
                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void CleanArgument(SocketAsyncEventArgs e, Session.SendContext context)
        {
            if (e != null)
            {
                e.SetBuffer(null, 0, 0);
            }

            if (context != null)
            {
                context.BytesTransferred = 0;
                context.SendData = null;
            }
        }

        internal event ExceptionSessionHandle OccurredExceptionFromSession;
        internal event DisconnectedSocketHandle DisconnectedSession;
        internal event SendCompletedHandle EventCompletedSendData;

        internal void SendAsync(Session session, IMessage message)
        {
            if (CheckSession(session) == false)
                return;
            if (CheckMessage(message) == false)
                return;
            SendMessageToSocketAsync(session, GetData(message));
        }

        private bool CheckMessage(IMessage message)
        {
            if (message == null
                || message.Header == null
                || message.Header.HeaderRawData == null
                || message.Body == null
                || message.Body.BodyRawData == null)
            {
                return false;
            }

            return true;
        }

        private byte[] GetData(IMessage message)
        {
            byte[] sendData = new byte[message.Header.HeaderRawData.Length + message.Body.BodyRawData.Length];
            message.Header.HeaderRawData.CopyTo(sendData, 0);
            message.Body.BodyRawData.CopyTo(sendData, message.Header.HeaderRawData.Length);

            return sendData;
        }

        private bool CheckSession(Session session)
        {
            if (session == null || session.Handle == null)
                return false;

            return true;
        }

        internal void SendAsync(IEnumerable<Session> sessionList, IMessage message)
        {
            bool validSession = sessionList.All(session => CheckSession(session));

            if (validSession == false)
                return;
            
            if (CheckMessage(message) == false)
                return;
            
            byte[] sendData = GetData(message);
            
            Parallel.ForEach(sessionList, session => 
            {
                SendMessageToSocketAsync(session, sendData);
            });
        }

        internal Task Send(Session session, IMessage message)
        {
            if (CheckSession(session) == false)
                return null;
            if (CheckMessage(message) == false)
                return null;

            byte[] sendData = GetData(message);
            return SendMessageToSocket(session, sendData, 0, sendData.Length);
        }

        private Task SendMessageToSocket(Session session, byte[] sendData, int offset, int size)
        {
            return Task.Factory.StartNew(() => 
            {
                int bytesTransfferred = 0;
                while (true)
                {
                    bytesTransfferred += session.Handle.Send(sendData, bytesTransfferred, sendData.Length - bytesTransfferred, SocketFlags.None);
                    if (bytesTransfferred >= sendData.Length)
                        break;
                }
            });
        }
    }
}
