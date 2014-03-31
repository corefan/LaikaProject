using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    /// <summary>
    /// Laika Tcp Server class
    /// </summary>
    /// <typeparam name="messageT">IMessage 인터페이스를 갖는 Type</typeparam>
    /// <typeparam name="headerT">IHeader 인터페이스를 갖는 Type</typeparam>
    /// <typeparam name="bodyT">IBody 인터페이스를 갖는 Type</typeparam>
    public class LaikaTcpServer<messageT, headerT, bodyT> : ILaikaServer
        where messageT : class, IMessage, new()
        where headerT : class, IHeader, new()
        where bodyT : class, IBody, new()
    {
        /// <summary>
        /// LaikaTcpServer 생성자
        /// </summary>
        /// <param name="port">Tcp 포트</param>
        public LaikaTcpServer(int port)
            : this(IPAddress.Any, port)
        {
            
        }
        /// <summary>
        /// LaikaTcpServer 생성자
        /// </summary>
        /// <param name="host">IP</param>
        /// <param name="port">Tcp 포트</param>
        public LaikaTcpServer(string host, int port)
            : this(IPAddress.Parse(host), port)
        {

        }
        /// <summary>
        /// LaikaTcpServer 생성자
        /// </summary>
        /// <param name="address">IPAddress 인스턴스</param>
        /// <param name="port">Tcp 포트</param>
        public LaikaTcpServer(IPAddress address, int port)
        {
            _endPoint = new IPEndPoint(address, port);
        }

        ~LaikaTcpServer()
        {
            Dispose(false);
        }
        /// <summary>
        /// 서버 준비. Nonblocking.
        /// </summary>
        public void NonblockingStart()
        {
            InitializeServer();
        }
        /// <summary>
        /// 서버 준비. Blocking.
        /// </summary>
        public void BlockingStart()
        {
            NonblockingStart();
            _serverWait.WaitOne();
        }
        /// <summary>
        /// Dispose 패턴. 서버 종료 시 호출이 필요합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void SendMessage(Session session, IMessage message)
        {
            _sender.SendAsync(session, message);
        }

        public void SendMessage(IEnumerable<Session> sessionList, IMessage message)
        {
            _sender.SendAsync(sessionList, message);
        }

        private void InitializeServer()
        {
            _listenerSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listenerSocket.LingerState.Enabled = false;
            _listenerSocket.Bind(_endPoint);
            _listenerSocket.Listen(1000);
            InitializeSender();
            InitializeReceiver();
            InitializeAcceptor();
        }

        private void InitializeReceiver()
        {
            _receiver = new Receiver<messageT, headerT, bodyT>();
            _receiver.ReceivedMessage += ReceivedMessage;
            _receiver.OccuredExceptionFromSession += OccuredExceptionFromSession;
            _receiver.DisconnectedSession += DisconnectedSession;
        }

        private void DisconnectedSession(object sender, DisconnectSocketEventArgs e)
        {
            if (e.SessionHandle.Handle != null)
            {
                e.SessionHandle.Handle.Close();
                e.SessionHandle.Handle.Dispose();
            }
        }

        private void OccuredExceptionFromSession(object sender, ExceptionFromSessionEventArgs e)
        {
            if (e.SessionHandle.Handle != null)
            {
                e.SessionHandle.Handle.Close();
                e.SessionHandle.Handle.Dispose();
            }

            this.OccuredError(this, new ExceptionEventArgs(e.Exception));
        }

        private void ReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            if (ReceivedMessageFromSession != null)
                ReceivedMessageFromSession(this, e);
        }
        private void InitializeSender()
        {
            _sender = new Sender<messageT, headerT, bodyT>();
            _sender.OccuredExceptionFromSession += OccuredExceptionFromSession;
            _sender.DisconnectedSession += DisconnectedSession;
        }

        private void InitializeAcceptor()
        {
            _acceptor = new Acceptor(_listenerSocket);
            _acceptor.ConnectedSession += ConnectedSession;
            _acceptor.OccuredExceptionFromAccept += OccuredExceptionFromAccept;
            _acceptor.NewAccept();
        }

        private void ConnectedSession(object sender, AcceptEventArgs e)
        {
            if (ConnectedSessionEvent != null)
                ConnectedSessionEvent(this, new ConnectedSessionEventArgs(e.SessionHandle));

            _receiver.BeginReceive(e.SessionHandle);
        }

        private void OccuredExceptionFromAccept(object sender, ExceptionEventArgs e)
        {
            if (OccuredError != null)
                OccuredError(this, e);
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
            if (_listenerSocket != null)
            {
                _listenerSocket.Shutdown(SocketShutdown.Both);
                _listenerSocket.Dispose();
                _listenerSocket = null;
            }
        }

        private IPEndPoint _endPoint;
        private Socket _listenerSocket;
        private ManualResetEvent _serverWait = new ManualResetEvent(false);
        private bool _disposed = false;
        private Acceptor _acceptor;
        private Receiver<messageT, headerT, bodyT> _receiver;
        private Sender<messageT, headerT, bodyT> _sender;

        public event ReceiveHandle ReceivedMessageFromSession;
        public event ErrorHandle OccuredError;
        public event ConnectHandle ConnectedSessionEvent;
    }
}
