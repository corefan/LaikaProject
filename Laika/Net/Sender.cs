using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        internal void SendAsync(IMessage message)
        {
            if (message == null)
                return;
            if (message.socket == null && (message.sockets == null || message.sockets.Count <= 0))
                return;
            if (message.Header == null || message.Header.HeaderRawData == null || message.Header.HeaderRawData.Length <= 0)
                return;
            if (message.Body == null || message.Body.BodyRawData == null || message.Body.BodyRawData.Length <= 0 || message.Body.BodyRawData.Length > LaikaConfig.MaxBodySize)
                return;

            byte[] sendData = new byte[message.Header.HeaderRawData.Length + message.Body.BodyRawData.Length];
            message.Header.HeaderRawData.CopyTo(sendData, 0);
            message.Body.BodyRawData.CopyTo(sendData, message.Header.HeaderRawData.Length);

            if (message.socket != null)
            {
                SendMessageToSocket(message.socket, sendData);
            }

            if (message.sockets != null && message.sockets.Count <= 0)
            {
                Parallel.ForEach(message.sockets, socket => 
                {
                    if (socket.Connected == true)
                        SendMessageToSocket(socket, sendData);
                });
                message.sockets.Clear();
            }
        }

        private void SendMessageToSocket(Socket socket, byte[] sendData)
        {
            SendContext context = new SendContext();
            context.SendData = sendData;

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(context.SendData, 0, context.SendData.Length);
            e.UserToken = context;
            e.Completed += SendCompleted;

            socket.SendAsync(e);
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = sender as Socket;
            SendContext context = e.UserToken as SendContext;

            try
            {
                if (socket == null)
                    throw new ArgumentNullException();

                int bytesTransferred = e.BytesTransferred;
                context.BytesTransferred += bytesTransferred;

                if (bytesTransferred <= 0)
                {
                    if (DisconnectedSocket != null)
                        DisconnectedSocket(socket);

                    CleanArgument(e, context);
                    return;
                }
                else if (context.BytesTransferred < context.SendData.Length)
                {
                    e.UserToken = context;
                    e.SetBuffer(context.SendData, context.BytesTransferred, context.SendData.Length - context.BytesTransferred);

                    socket.SendAsync(e);
                }
                else if (context.BytesTransferred == context.SendData.Length)
                {
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
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(socket, ex);
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

        internal event ExceptionSocketHandle OccuredExceptionFromSocket;
        internal delegate void ExceptionSocketHandle(Socket socket, Exception e);

        internal event DisconnectedSocketHandle DisconnectedSocket;
        internal delegate void DisconnectedSocketHandle(Socket socket);

        internal class SendContext
        {
            internal int BytesTransferred { get; set; }
            internal byte[] SendData { get; set; }
        }
    }
}
