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
        internal void ConnectAsync(Session session, IPEndPoint endPoint)
        {
            if (session == null || session.Handle == null || endPoint == null)
                throw new ArgumentNullException();

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.RemoteEndPoint = endPoint;
            e.Completed += ConnectCompleted;
            e.UserToken = session;
            session.Handle.ConnectAsync(e);
        }

        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            CleanArgument(e);
            if (ConnectedSession != null)
                ConnectedSession(this, new ConnectedSessionEventArgs(session));
        }

        private void CleanArgument(SocketAsyncEventArgs e)
        {
            if (e != null)
            {
                e.Dispose();
                e = null;
            }
        }

        internal event ConnectHandle ConnectedSession;
        internal delegate void ConnectHandle(object sender, ConnectedSessionEventArgs e);
    }
}
