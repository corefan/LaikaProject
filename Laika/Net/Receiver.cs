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
        internal delegate void ReceiveHandle(Socket socket, IMessage message);

        internal event ExceptionSocketHandle OccuredExceptionFromSocket;
        internal delegate void ExceptionSocketHandle(Socket socket, Exception ex);

        internal event DisconnectedSocketHandle DisconnectedSocket;
        internal delegate void DisconnectedSocketHandle(Socket socket);

        internal void BeginReceive(Socket socket)
        {
            if (socket == null)
                return;
            try
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                messageT message = new messageT();
                message.Header = new headerT();
                message.Header.HeaderRawData = new byte[message.Header.GetHeaderSize()];
                e.UserToken = message;
                e.SetBuffer(message.Header.HeaderRawData, 0, message.Header.HeaderRawData.Length);
                e.Completed += ReceiveHeaderCompleted;

                socket.ReceiveAsync(e);
            }
            catch (Exception ex)
            {
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
            }
        }

        private void ReceiveHeaderCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = (Socket)sender;
            messageT message = (messageT)e.UserToken;
            try
            {
                if (socket == null)
                    throw new ArgumentNullException();

                int transferred = e.BytesTransferred;
                if (transferred <= 0)
                {
                    if (DisconnectedSocket != null)
                        DisconnectedSocket(socket);
                    return;
                }
                else if (message.Header.BytesTransferred + transferred < message.Header.GetHeaderSize())
                {
                    message.Header.BytesTransferred += transferred;
                    SocketAsyncEventArgs ea = new SocketAsyncEventArgs();
                    ea.UserToken = message;
                    ea.SetBuffer(message.Header.HeaderRawData, message.Header.BytesTransferred, message.Header.GetHeaderSize() - message.Header.BytesTransferred);
                    ea.Completed += ReceiveHeaderCompleted;

                    socket.ReceiveAsync(ea);
                }
                else if (message.Header.BytesTransferred + transferred == message.Header.GetHeaderSize())
                {
                    message.Body = new bodyT();
                    message.Header.ContentsSize = BitConverter.ToInt32(message.Header.HeaderRawData, 0);
                    message.Body.BodyRawData = new byte[message.Header.ContentsSize];
                    SocketAsyncEventArgs ea = new SocketAsyncEventArgs();
                    ea.UserToken = message;
                    ea.SetBuffer(message.Body.BodyRawData, 0, message.Header.ContentsSize);
                    ea.Completed += ReceiveBodyCompleted;

                    socket.ReceiveAsync(ea);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception ex)
            {
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
            }
        }

        private void ReceiveBodyCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = (Socket)sender;
            messageT message = (messageT)e.UserToken;
            try
            {
                if (socket == null)
                    throw new ArgumentNullException();

                int transferred = e.BytesTransferred;
                if (transferred <= 0)
                {
                    if (DisconnectedSocket != null)
                        DisconnectedSocket(socket);
                    return;
                }
                else if (message.Body.BytesTransferred + transferred < message.Header.ContentsSize)
                {
                    message.Body.BytesTransferred += transferred;
                    SocketAsyncEventArgs ea = new SocketAsyncEventArgs();
                    ea.UserToken = message;
                    ea.SetBuffer(message.Body.BodyRawData, message.Body.BytesTransferred, message.Header.ContentsSize - message.Body.BytesTransferred);
                    ea.Completed += ReceiveBodyCompleted;

                    socket.ReceiveAsync(ea);
                }
                else if (message.Body.BytesTransferred + transferred == message.Header.ContentsSize)
                {
                    if (ReceivedMessage != null)
                        ReceivedMessage(socket, message);
                }
                else
                {
                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
            }
        }
    }
}
