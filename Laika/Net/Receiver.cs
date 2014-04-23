using System;
using System.Net.Sockets;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    internal class Receiver
    {
        internal event ReceiveHandle ReceivedMessage;
        internal event ExceptionSessionHandle OccurredExceptionFromSession;
        internal event DisconnectedSocketHandle DisconnectedSession;
        private ILaikaNet _net;
        internal Receiver(ILaikaNet net)
        {
            _net = net;
        }

        internal void ReceiveAsync(Session session)
        {
            if (session == null || session.Handle == null)
                return;

            try
            {
                IMessage message = _net.MessageFactory();

                message.Session = session;
                message.Header.HeaderRawData = new byte[message.Header.GetHeaderSize()];
                session.ReceiveEventArgs.UserToken = message;
                session.ReceiveEventArgs.SetBuffer(message.Header.HeaderRawData, 0, message.Header.HeaderRawData.Length);
                session.ReceiveEventArgs.Completed += ReceiveHeaderCompleted;

                if (session.Handle.ReceiveAsync(session.ReceiveEventArgs) == false)
                    ReceiveHeaderCompleted(session.Handle, session.ReceiveEventArgs);
            }
            catch (Exception ex)
            {
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void ReceiveHeaderCompleted(object sender, SocketAsyncEventArgs e)
        {
            IMessage message = e.UserToken as IMessage;
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
                    
                    return;
                }
                else if (message.Header.BytesTransferred < message.Header.GetHeaderSize())
                {
                    e.SetBuffer(message.Header.HeaderRawData, message.Header.BytesTransferred, message.Header.GetHeaderSize() - message.Header.BytesTransferred);

                    if (session.Handle.ReceiveAsync(e) == false)
                        ReceiveHeaderCompleted(session.Handle, e);
                }
                else if (message.Header.BytesTransferred == message.Header.GetHeaderSize())
                {
                    message.Header.ContentsSize = BitConverter.ToInt32(message.Header.HeaderRawData, 0);
                    if (message.Header.ContentsSize <= 0 || message.Header.ContentsSize > LaikaConfig.MaxBodySize)
                        throw new ArgumentException(string.Format("Invalid Body Size, Size : {0}", message.Header.ContentsSize));

                    message.Body.BodyRawData = new byte[message.Header.ContentsSize];

                    e.SetBuffer(message.Body.BodyRawData, 0, message.Header.ContentsSize);
                    e.Completed -= ReceiveHeaderCompleted;
                    e.Completed += ReceiveBodyCompleted;

                    if (session.Handle.ReceiveAsync(e) == false)
                        ReceiveBodyCompleted(session.Handle, e);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception ex)
            {
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }

        private void ReceiveBodyCompleted(object sender, SocketAsyncEventArgs e)
        {
            IMessage message = e.UserToken as IMessage;
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
                    
                    return;
                }
                else if (message.Body.BytesTransferred < message.Header.ContentsSize)
                {
                    e.SetBuffer(message.Body.BodyRawData, message.Body.BytesTransferred, message.Header.ContentsSize - message.Body.BytesTransferred);

                    if (session.Handle.ReceiveAsync(e) == false)
                        ReceiveBodyCompleted(session.Handle, e);
                }
                else if (message.Body.BytesTransferred == message.Header.ContentsSize)
                {
                    e.Completed -= ReceiveBodyCompleted;
                    if (ReceivedMessage != null)
                        ReceivedMessage(this, new ReceivedMessageEventArgs(message));
                    
                    ReceiveAsync(session);
                }
                else
                {
                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                if (OccurredExceptionFromSession != null)
                    OccurredExceptionFromSession(this, new ExceptionFromSessionEventArgs(session, ex));
            }
        }
    }
}
