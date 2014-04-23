using System;
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
            session.ServerSequenceId = Laika.UIDGen.UniqueIDGenerator.GetID();
            SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.UserToken = session;
            acceptArgs.Completed += AcceptCompleted;
            try
            {
                if (_acceptingSocket.AcceptAsync(acceptArgs) == false)
                    AcceptCompleted(_acceptingSocket, acceptArgs);
            }
            catch (Exception ex)
            {
                if (OccurredExceptionFromAccept != null)
                    OccurredExceptionFromAccept(this, new ExceptionEventArgs(session, ex));
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
        internal event ErrorHandle OccurredExceptionFromAccept;
        
    }
}
