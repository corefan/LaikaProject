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
            _sender.SendAsync(_session, message);
        }

        private void InitializeClient()
        {
            _session = new Session();
            _session.Handle = new Socket(_remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            InitializeConnector();
            InitializeSender();
            InitializeReceiver();
        }

        private void InitializeReceiver()
        {
            _receiver = new Receiver<messageT, headerT, bodyT>();
            _receiver.DisconnectedSession += DisconnectedSession;
            _receiver.OccuredExceptionFromSession += OccuredExceptionFromSession;
            _receiver.ReceivedMessage += ReceivedMessageFromServer;
            _receiver.BeginReceive(_session);
        }

        private void OccuredExceptionFromSession(object sender, ExceptionFromSessionEventArgs e)
        {
            if (e.SessionHandle.Handle != null)
            {
                e.SessionHandle.Handle.Close();
                e.SessionHandle.Handle.Dispose();
            }
            if (OccuredException != null)
                OccuredException(this, e);
        }

        private void DisconnectedSession(object sender, DisconnectSocketEventArgs e)
        {
            if (DisconnectedSessionEvent != null)
                DisconnectedSessionEvent(this, e);
        }

        private void ReceivedMessageFromServer(object sender, ReceivedMessageEventArgs e)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, e);
        }
                                
        private void InitializeSender()
        {
            _sender = new Sender<messageT, headerT, bodyT>();
            _sender.DisconnectedSession += DisconnectedSession;
            _sender.OccuredExceptionFromSession += OccuredExceptionFromSession;
        }

        private void InitializeConnector()
        {
            _connector = new Connector();
            _connector.ConnectedSession += ConnectedSession;
            _connector.ConnectAsync(_session, _remoteEndPoint);
        }

        private void ConnectedSession(object sender, ConnectedSessionEventArgs e)
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
            if (_session != null && _session.Handle != null)
            {
                _session.Handle.Shutdown(SocketShutdown.Both);
                _session.Handle.Dispose();
            }
        }

        private void InitializeEndPoint(string host, int port)
        {
            IPAddress address = IPAddress.Parse(host);
            _remoteEndPoint = new IPEndPoint(address, port);
        }

        private Session _session;
        //private Socket _socket;
        private IPEndPoint _remoteEndPoint;
        private bool _disposed = false;
        private ManualResetEvent _clientWait = new ManualResetEvent(false);
        private ManualResetEvent _connectWait = new ManualResetEvent(false);
        private Connector _connector;
        private Sender<messageT, headerT, bodyT> _sender;
        private Receiver<messageT, headerT, bodyT> _receiver;

        public event DisconnectedSocketHandle DisconnectedSessionEvent;
        public delegate void DisconnectedSocketHandle(object sender, DisconnectSocketEventArgs e);

        public event SocketExceptionHandle OccuredException;
        public delegate void SocketExceptionHandle(object sender, ExceptionFromSessionEventArgs e);

        public event ReceiveHandle ReceivedMessage;
        public delegate void ReceiveHandle(object sender, ReceivedMessageEventArgs e);
    }
}
