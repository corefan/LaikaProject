using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        internal delegate void ReceiveHandle(IMessage message);

        internal event ExceptionSocketHandle OccuredExceptionFromSocket;
        internal delegate void ExceptionSocketHandle(Socket socket, Exception ex);

        internal event DisconnectedSocketHandle DisconnectedSocket;
        internal delegate void DisconnectedSocketHandle(Socket socket);

        internal void BeginReceive(Socket socket)
        {
            if (socket == null)
                return;
            SocketAsyncEventArgs e = null;
            try
            {
                e = new SocketAsyncEventArgs();
                messageT message = new messageT();
                message.socket = socket;
                message.Header = new headerT();
                message.Header.HeaderRawData = new byte[message.Header.GetHeaderSize()];
                e.UserToken = message;
                e.SetBuffer(message.Header.HeaderRawData, 0, message.Header.HeaderRawData.Length);
                e.Completed += ReceiveHeaderCompleted;

                socket.ReceiveAsync(e);
            }
            catch (Exception ex)
            {
                CleanArgument(e);
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
            }
        }

        private void ReceiveHeaderCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = sender as Socket;
            messageT message = e.UserToken as messageT;
            try
            {
                if (socket == null)
                    throw new ArgumentNullException();

                int transferred = e.BytesTransferred;
                message.Header.BytesTransferred += transferred;
                
                if (transferred <= 0)
                {
                    if (DisconnectedSocket != null)
                        DisconnectedSocket(socket);
                    
                    CleanArgument(e);
                    return;
                }
                else if (message.Header.BytesTransferred < message.Header.GetHeaderSize())
                {
                    e.SetBuffer(message.Header.HeaderRawData, message.Header.BytesTransferred, message.Header.GetHeaderSize() - message.Header.BytesTransferred);

                    socket.ReceiveAsync(e);
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

                    socket.ReceiveAsync(e);
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
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
            }
        }

        private void ReceiveBodyCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = sender as Socket;
            messageT message = e.UserToken as messageT;
            try
            {
                if (socket == null)
                    throw new ArgumentNullException();

                int transferred = e.BytesTransferred;
                message.Body.BytesTransferred += transferred;

                if (transferred <= 0)
                {
                    if (DisconnectedSocket != null)
                        DisconnectedSocket(socket);
                    
                    CleanArgument(e);
                    return;
                }
                else if (message.Body.BytesTransferred < message.Header.ContentsSize)
                {
                    e.SetBuffer(message.Body.BodyRawData, message.Body.BytesTransferred, message.Header.ContentsSize - message.Body.BytesTransferred);

                    socket.ReceiveAsync(e);
                }
                else if (message.Body.BytesTransferred == message.Header.ContentsSize)
                {
                    CleanArgument(e);

                    if (ReceivedMessage != null)
                        ReceivedMessage(message);

                    BeginReceive(socket);
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
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
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
