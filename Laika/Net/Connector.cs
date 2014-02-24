using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Laika.Net
{
    internal class Connector
    {
        internal void ConnectAsync(Socket socket, IPEndPoint endPoint)
        {
            if (socket == null || endPoint == null)
                throw new ArgumentNullException();

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.RemoteEndPoint = endPoint;
            e.Completed += ConnectCompleted;

            socket.ConnectAsync(e);
        }

        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            CleanArgument(e);
            if (ConnectedSocket != null)
                ConnectedSocket();
        }

        private void CleanArgument(SocketAsyncEventArgs e)
        {
            if (e != null)
            {
                e.Dispose();
                e = null;
            }
        }

        internal event ConnectHandle ConnectedSocket;
        internal delegate void ConnectHandle();
    }
}
