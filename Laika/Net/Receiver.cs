using System;
using System.Net.Sockets;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    internal class Receiver<messageT, headerT, bodyT>
        where messageT : class, IMessage, new()
        where headerT : class, IHeader, new()
        where bodyT : class, IBody, new()
    {
        internal event ReceiveHandle ReceivedMessage;
        internal event ExceptionSessionHandle OccurredExceptionFromSession;
        internal event DisconnectedSocketHandle DisconnectedSession;

        internal void BeginReceive(Session session)
        {
            if (session == null || session.Handle == null)
                return;

            SocketAsyncEventArgs receiveEventArg = null;
            try
            {
                receiveEventArg = new SocketAsyncEventArgs();
                messageT message = new messageT();
                message.Session = session;
                message.Header = new headerT();
                message.Header.HeaderRawData = new byte[message.Header.GetHeaderSize()];
                receiveEventArg.UserToken = message;
                receiveEventArg.SetBuffer(message.Header.HeaderRawData, 0, message.Header.HeaderRawData.Length);
                receiveEventArg.Completed += ReceiveHeaderCompleted;

                session.Handle.ReceiveAsync(receiveEventArg);
            }
            catch (Exception ex)
            {
                CleanArgument(receiveEventArg);
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void ReceiveHeaderCompleted(object sender, SocketAsyncEventArgs e)
        {
            messageT message = e.UserToken as messageT;
            Session session = message.Session;
            try
            {
                if (session.Handle == null)
                    throw new ArgumentNullException();

                int transferred = e.BytesTransferred;
                message.Header.BytesTransferred += transferred;
                
                if (transferred <= 0)
                {
                    if (DisconnectedSession != null)
                        DisconnectedSession(this, new DisconnectSocketEventArgs(session));
                    
                    CleanArgument(e);
                    return;
                }
                else if (message.Header.BytesTransferred < message.Header.GetHeaderSize())
                {
                    e.SetBuffer(message.Header.HeaderRawData, message.Header.BytesTransferred, message.Header.GetHeaderSize() - message.Header.BytesTransferred);

                    session.Handle.ReceiveAsync(e);
                }
                else if (message.Header.BytesTransferred == message.Header.GetHeaderSize())
                {
                    message.Body = new bodyT();
                    message.Header.ContentsSize = BitConverter.ToInt32(message.Header.HeaderRawData, 0);
                    if (message.Header.ContentsSize <= 0 || message.Header.ContentsSize > LaikaConfig.MaxBodySize)
                        throw new ArgumentException(string.Format("Invalid Body Size, Size : {0}", message.Header.ContentsSize));

                    message.Body.BodyRawData = new byte[message.Header.ContentsSize];

                    e.SetBuffer(message.Body.BodyRawData, 0, message.Header.ContentsSize);
                    e.Completed -= ReceiveHeaderCompleted;
                    e.Completed += ReceiveBodyCompleted;

                    session.Handle.ReceiveAsync(e);
                }
                else
                {
                    CleanArgument(e);
                    throw new ArgumentException();
                }
            }
            catch (Exception ex)
            {
                CleanArgument(e);
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void ReceiveBodyCompleted(object sender, SocketAsyncEventArgs e)
        {
            messageT message = e.UserToken as messageT;
            Session session = message.Session;
            try
            {
                if (session == null || session.Handle == null)
                    throw new ArgumentNullException();

                int transferred = e.BytesTransferred;
                message.Body.BytesTransferred += transferred;

                if (transferred <= 0)
                {
                    if (DisconnectedSession != null)
                        DisconnectedSession(this, new DisconnectSocketEventArgs(session));
                    
                    CleanArgument(e);
                    return;
                }
                else if (message.Body.BytesTransferred < message.Header.ContentsSize)
                {
                    e.SetBuffer(message.Body.BodyRawData, message.Body.BytesTransferred, message.Header.ContentsSize - message.Body.BytesTransferred);

                    session.Handle.ReceiveAsync(e);
                }
                else if (message.Body.BytesTransferred == message.Header.ContentsSize)
                {
                    CleanArgument(e);

                    if (ReceivedMessage != null)
                        ReceivedMessage(this, new ReceivedMessageEventArgs(message));

                    BeginReceive(session);
                }
                else
                {
                    CleanArgument(e);
                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                CleanArgument(e);
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void CleanArgument(SocketAsyncEventArgs e)
        {
            if (e != null)
            {
                e.Dispose();
                e = null;
            }
        }
    }
}
