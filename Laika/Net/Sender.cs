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
    internal class Sender<messageT, headerT, bodyT>
        where messageT : class, IMessage, new()
        where headerT : class, IHeader, new()
        where bodyT : class, IBody, new()
    {
        private void SendMessageToSocket(Session session, byte[] sendData)
        {
            SendContext context = new SendContext();
            context.SendData = sendData;
            context.Session = session;

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(context.SendData, 0, context.SendData.Length);
            e.UserToken = context;
            e.Completed += SendCompleted;

            session.Handle.SendAsync(e);
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            SendContext context = e.UserToken as SendContext;
            Session session = context.Session;
            try
            {
                if (session.Handle == null)
                    throw new ArgumentNullException();

                int bytesTransferred = e.BytesTransferred;
                context.BytesTransferred += bytesTransferred;

                if (bytesTransferred <= 0)
                {
                    if (DisconnectedSession != null)
                        DisconnectedSession(this, new DisconnectSocketEventArgs(session));

                    CleanArgument(e, context);
                    return;
                }
                else if (context.BytesTransferred < context.SendData.Length)
                {
                    e.UserToken = context;
                    e.SetBuffer(context.SendData, context.BytesTransferred, context.SendData.Length - context.BytesTransferred);

                    session.Handle.SendAsync(e);
                }
                else if (context.BytesTransferred == context.SendData.Length)
                {
                    if (EventCompletedSendData != null)
                        EventCompletedSendData(this, new SendMessageEventArgs(session));
                    CleanArgument(e, context);
                }
                else
                {
                    CleanArgument(e, context);
                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void CleanArgument(SocketAsyncEventArgs e, SendContext context)
        {
            if (e != null)
            {
                e.Dispose();
                e = null;
            }

            if (context != null)
            {
                context.SendData = null;
                context = null;
            }
        }

        internal event ExceptionSessionHandle OccurredExceptionFromSession;
        internal event DisconnectedSocketHandle DisconnectedSession;
        internal event SendCompletedHandle EventCompletedSendData;

        internal class SendContext
        {
            internal int BytesTransferred { get; set; }
            internal byte[] SendData { get; set; }
            internal Session Session { get; set; }
        }

        internal void SendAsync(Session session, IMessage message)
        {
            if (CheckSession(session) == false)
                return;
            if (CheckMessage(message) == false)
                return;
            SendMessageToSocket(session, GetData(message));
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
                SendMessageToSocket(session, sendData);
            });
        }
    }
}
