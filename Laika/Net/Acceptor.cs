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
            SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCompleted);
            try
            {
                _acceptingSocket.AcceptAsync(acceptArgs);
            }
            catch (Exception ex)
            {
                if (OccuredExceptionFromSocket != null)
                    OccuredExceptionFromSocket(ex);
            }
        }

        internal void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket client = e.AcceptSocket;
            NewAccept();

            if (ConnectedClient != null)
                ConnectedClient(client);
        }

        private Socket _acceptingSocket;

        internal event ConnectedHandle ConnectedClient;
        internal delegate void ConnectedHandle(Socket socket);

        internal event ExceptionSocketHandle OccuredExceptionFromSocket;
        internal delegate void ExceptionSocketHandle(Exception ex);
    }
}
