using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Laika.Net
{
    internal class Acceptor
    {
        internal Acceptor(Socket socket)
        {
            if (socket == null)
                throw new ArgumentNullException();
            
            _acceptingSocket = socket;
        }
        internal void NewAccept()
        {
            Session session = new Session();
            SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.UserToken = session;
            acceptArgs.Completed += AcceptCompleted;
            try
            {
                _acceptingSocket.AcceptAsync(acceptArgs);
            }
            catch (Exception ex)
            {
                if (OccuredExceptionFromAccept != null)
                    OccuredExceptionFromAccept(this, new ExceptionEventArgs(ex));
            }
        }

        internal void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            Session session = (Session)e.UserToken;
            session.Handle = e.AcceptSocket;
            NewAccept();

            CleanArgument(e);
            if (ConnectedSession != null)
                ConnectedSession(this, new AcceptEventArgs(session));
        }

        private void CleanArgument(SocketAsyncEventArgs e)
        {
            if (e != null)
            {
                e.Dispose();
                e = null;
            }
        }

        private Socket _acceptingSocket;

        internal event ConnectedHandle ConnectedSession;
        internal delegate void ConnectedHandle(object sender, AcceptEventArgs e);

        internal event ExceptionSocketHandle OccuredExceptionFromAccept;
        internal delegate void ExceptionSocketHandle(object sender, ExceptionEventArgs e);
    }
}
