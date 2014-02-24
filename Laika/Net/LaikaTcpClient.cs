using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    public class LaikaTcpClient<messageT, headerT, bodyT> : IDisposable
        where messageT : class, IMessage, new()
        where headerT : class, IHeader, new()
        where bodyT : class, IBody, new()
    {
        public LaikaTcpClient(string host, int port)
        {
            InitializeEndPoint(host, port);
        }

        public void Run()
        {
            Poll();
            _clientWait.WaitOne();
        }

        public void Poll()
        {
            InitializeClient();
        }

        public void SendAsync(IMessage message)
        {
            _connectWait.WaitOne();
            message.socket = _socket;
            _sender.SendAsync(message);
        }

        private void InitializeClient()
        {
            _socket = new Socket(_remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            InitializeConnector();
            InitializeSender();
            InitializeReceiver();
        }

        private void InitializeReceiver()
        {
            _receiver = new Receiver<messageT, headerT, bodyT>();
            _receiver.DisconnectedSocket += DisconnectedSocketFromServer;
            _receiver.OccuredExceptionFromSocket += OccuredExceptionFromSocket;
            _receiver.ReceivedMessage += ReceivedMessageFromServer;
            _receiver.BeginReceive(_socket);
        }

        private void ReceivedMessageFromServer(IMessage message)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(message);
        }

        

        private void OccuredExceptionFromSocket(Socket socket, Exception e)
        {
            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
            }

            if (OccuredException != null)
                OccuredException(e);
        }

        private void DisconnectedSocketFromServer(Socket socket)
        {
            if (DisconnectedSocket != null)
                DisconnectedSocket(socket);
        }

        private void InitializeSender()
        {
            _sender = new Sender<messageT, headerT, bodyT>();
            _sender.DisconnectedSocket += DisconnectedSocketFromServer;
            _sender.OccuredExceptionFromSocket += OccuredExceptionFromSocket;
        }

        private void InitializeConnector()
        {
            _connector = new Connector();
            _connector.ConnectedSocket += ConnectedSocket;
            _connector.ConnectAsync(_socket, _remoteEndPoint);
        }

        private void ConnectedSocket()
        {
            _connectWait.Set();
        }

        ~LaikaTcpClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;
            if (disposing == true)
            {
                Clear();
            }
            _disposed = true;
        }

        private void Clear()
        {
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Dispose();
            }
        }

        private void InitializeEndPoint(string host, int port)
        {
            IPAddress address = IPAddress.Parse(host);
            _remoteEndPoint = new IPEndPoint(address, port);
        }

        private Socket _socket;
        private IPEndPoint _remoteEndPoint;
        private bool _disposed = false;
        private ManualResetEvent _clientWait = new ManualResetEvent(false);
        private ManualResetEvent _connectWait = new ManualResetEvent(false);
        private Connector _connector;
        private Sender<messageT, headerT, bodyT> _sender;
        private Receiver<messageT, headerT, bodyT> _receiver;

        public event DisconnectedSocketHandle DisconnectedSocket;
        public delegate void DisconnectedSocketHandle(Socket socket);

        public event SocketExceptionHandle OccuredException;
        public delegate void SocketExceptionHandle(Exception ex);

        public event ReceiveHandle ReceivedMessage;
        public delegate void ReceiveHandle(IMessage message);
    }
}
