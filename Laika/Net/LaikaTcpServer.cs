using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            _sender.Send(session, message);
        }
            
        public void SendMessageAsync(Session session, IMessage message)
        {
            _sender.SendAsync(session, message);
        }

        public void SendMessageAsync(IEnumerable<Session> sessionList, IMessage message)
        {
            _sender.SendAsync(sessionList, message);
        }

        public void ReleaseBlocking()
        {
            _serverWait.Set();
        }

        public void SessionCleanup(TimeSpan span)
        {
            IEnumerable<Session> sessions = _sessionDictionary.Where(x => 
            {
                if (x.Value.LastReceivedTime + span < DateTime.Now)
                    return true;
                return false;
            }).Select(x=>x.Value);

            foreach (var s in sessions)
            {
                Session session;
                _sessionDictionary.TryRemove(s.UniqueId, out session);
                session.Dispose();
            }
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
            _receiver = new Receiver(this);
            _receiver.ReceivedMessage += ReceivedMessage;
            _receiver.OccurredExceptionFromSession += OccurredExceptionFromSession;
            _receiver.DisconnectedSession += DisconnectedSession;
        }

        private void DisconnectedSession(object sender, DisconnectSocketEventArgs e)
        {
            Session session;
            _sessionDictionary.TryRemove(e.SessionHandle.UniqueId, out session);

            if (Disconnect != null)
            {
                Disconnect(this, e);
            }

            if (e.SessionHandle.Handle != null)
            {
                e.SessionHandle.Dispose();
            }
        }

        private void OccurredExceptionFromSession(object sender, ExceptionFromSessionEventArgs e)
        {
            Session session;
            _sessionDictionary.TryRemove(e.SessionHandle.UniqueId, out session);

            if (OccurredError != null)
                OccurredError(this, new ExceptionEventArgs(e.SessionHandle, e.Exception));

            if (e.SessionHandle.Handle != null)
            {
                e.SessionHandle.Dispose();
            }
        }

        private void ReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            e.Message.Session.LastReceivedTime = DateTime.Now;

            if (ReceivedMessageFromSession != null)
                ReceivedMessageFromSession(this, e);
        }
        private void InitializeSender()
        {
            _sender = new Sender();
            _sender.OccurredExceptionFromSession += OccurredExceptionFromSession;
            _sender.DisconnectedSession += DisconnectedSession;
            _sender.EventCompletedSendData += EventCompletedSendDataProc;
        }

        private void EventCompletedSendDataProc(object sender, SendMessageEventArgs e)
        {
            if (EventCompletedSendData != null)
                EventCompletedSendData(this, e);
        }

        private void InitializeAcceptor()
        {
            _acceptor = new Acceptor(_listenerSocket);
            _acceptor.ConnectedSession += ConnectedSession;
            _acceptor.OccurredExceptionFromAccept += OccurredExceptionFromAccept;
            _acceptor.NewAccept();
        }

        private void ConnectedSession(object sender, AcceptEventArgs e)
        {
            e.SessionHandle.LastReceivedTime = DateTime.Now;
            _sessionDictionary.TryAdd(e.SessionHandle.UniqueId, e.SessionHandle);

            if (ConnectedSessionEvent != null)
                ConnectedSessionEvent(this, new ConnectedSessionEventArgs(e.SessionHandle));

            _receiver.ReceiveAsync(e.SessionHandle);
        }

        private void OccurredExceptionFromAccept(object sender, ExceptionEventArgs e)
        {
            if (OccurredError != null)
                OccurredError(this, e);
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
                _listenerSocket.Close();
                _listenerSocket.Dispose();
                _listenerSocket = null;
            }
        }

        public IMessage MessageFactory()
        {
            IMessage message = new messageT();
            message.Header = new headerT();
            message.Body = new bodyT();
            return message;
        }

        private IPEndPoint _endPoint;
        private Socket _listenerSocket;
        private ManualResetEvent _serverWait = new ManualResetEvent(false);
        private bool _disposed = false;
        private Acceptor _acceptor;
        private Receiver _receiver;
        private Sender _sender;
        private ConcurrentDictionary<long, Session> _sessionDictionary = new ConcurrentDictionary<long, Session>();

        public event ReceiveHandle ReceivedMessageFromSession;
        public event ErrorHandle OccurredError;
        public event ConnectHandle ConnectedSessionEvent;
        public event DisconnectedSocketHandle Disconnect;
        public event SendCompletedHandle EventCompletedSendData;
    }
}
