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
    internal class Sender<messageT, headerT, bodyT>
        where messageT : class, IMessage, new()
        where headerT : class, IHeader, new()
        where bodyT : class, IBody, new()
    {
        internal void SendAsync(IMessage message)
        {
            if (message == null)
                return;
            if (message.socket == null)
                return;
            if (message.Header == null || message.Header.HeaderRawData == null || message.Header.HeaderRawData.Length <= 0)
                return;
            if (message.Body == null || message.Body.BodyRawData == null || message.Body.BodyRawData.Length <= 0)
                return;

            byte[] sendData = new byte[message.Header.HeaderRawData.Length + message.Body.BodyRawData.Length];
            message.Header.HeaderRawData.CopyTo(sendData, 0);
            message.Body.BodyRawData.CopyTo(sendData, message.Header.HeaderRawData.Length);

            SendContext context = new SendContext();
            context.SendData = sendData;

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(sendData, 0, sendData.Length);
            e.UserToken = context;
            e.Completed += SendCompleted;

            message.socket.SendAsync(e);
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = (Socket)sender;
            SendContext context = (SendContext)e.UserToken;

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
                }
                else if (context.BytesTransferred < context.SendData.Length)
                {
                    SocketAsyncEventArgs ea = new SocketAsyncEventArgs();
                    ea.UserToken = context;
                    ea.SetBuffer(context.SendData, context.BytesTransferred, context.SendData.Length - context.BytesTransferred);
                    ea.Completed += SendCompleted;

                    socket.SendAsync(ea);
                }
                else if (context.BytesTransferred == context.SendData.Length)
                {
                    // send completed.
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
