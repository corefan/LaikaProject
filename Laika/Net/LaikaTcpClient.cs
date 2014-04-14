using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    public class LaikaTcpClient<messageT, headerT, bodyT> : ILaikaClient
        where messageT : class, IMessage, new()
        where headerT : class, IHeader, new()
        where bodyT : class, IBody, new()
    {
        public LaikaTcpClient(string host, int port)
        {
            InitializeEndPoint(host, port);
        }

        public void BlockingStart()
        {
            NonBlockingStart();
            _clientWait.WaitOne();
        }

        public void NonBlockingStart()
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
            _receiver.OccurredExceptionFromSession += OccurredExceptionFromSession;
            _receiver.ReceivedMessage += ReceivedMessageFromServer;
            _receiver.BeginReceive(_session);
        }

        private void OccurredExceptionFromSession(object sender, ExceptionFromSessionEventArgs e)
        {
            if (e.SessionHandle.Handle != null)
            {
                e.SessionHandle.Dispose();
            }
            if (OccurredException != null)
                OccurredException(this, e);
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
            _sender.OccurredExceptionFromSession += OccurredExceptionFromSession;
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
            if (ConnectedEvent != null)
                ConnectedEvent(this, e);
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
                _session.Dispose();
            }
        }

        private void InitializeEndPoint(string host, int port)
        {
            IPAddress address = IPAddress.Parse(host);
            _remoteEndPoint = new IPEndPoint(address, port);
        }

        private Session _session;
        private IPEndPoint _remoteEndPoint;
        private bool _disposed = false;
        private ManualResetEvent _clientWait = new ManualResetEvent(false);
        private ManualResetEvent _connectWait = new ManualResetEvent(false);
        private Connector _connector;
        private Sender<messageT, headerT, bodyT> _sender;
        private Receiver<messageT, headerT, bodyT> _receiver;

        public event DisconnectedSocketHandle DisconnectedSessionEvent;
        public event SocketExceptionHandle OccurredException;
        public event ReceiveHandle ReceivedMessage;
        public event ConnectHandle ConnectedEvent;
    }
}
