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
    /// <summary>
    /// Laika Tcp Server class
    /// </summary>
    /// <typeparam name="messageT">IMessage 인터페이스를 갖는 Type</typeparam>
    /// <typeparam name="headerT">IHeader 인터페이스를 갖는 Type</typeparam>
    /// <typeparam name="bodyT">IBody 인터페이스를 갖는 Type</typeparam>
    public class LaikaTcpServer<messageT, headerT, bodyT> : IDisposable
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
        public void Poll()
        {
            InitializeServer();
        }
        /// <summary>
        /// 서버 준비. Blocking.
        /// </summary>
        public void Run()
        {
            Poll();
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
        public void SendMessage(IMessage message)
        {
            _sender.SendAsync(message);
        }

        private void InitializeServer()
        {
            _serverSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _serverSocket.LingerState.Enabled = false;
            _serverSocket.Bind(_endPoint);
            _serverSocket.Listen(1000);
            InitializeSender();
            InitializeReceiver();
            InitializeAcceptor();
        }

        private void InitializeReceiver()
        {
            _receiver = new Receiver<messageT, headerT, bodyT>();
            _receiver.ReceivedMessage += ReceivedMessage;
            _receiver.OccuredExceptionFromSocket += OccuredExceptionClient;
            _receiver.DisconnectedSocket += DisconnectedSocket;
        }

        private void DisconnectedSocket(Socket client)
        {
            if (client != null)
            {
                client.Close();
                client.Dispose();
            }
        }
        private void OccuredExceptionClient(Socket client, Exception ex)
        {
            if (client != null)
            {
                client.Close();
                client.Dispose();
            }

            OccuredExceptionSocket(ex);
        }
        private void ReceivedMessage(IMessage message)
        {
            if (ReceivedMessageFromClient != null)
                ReceivedMessageFromClient(message.socket, message);
        }
        private void InitializeSender()
        {
            _sender = new Sender<messageT, headerT, bodyT>();
            _sender.OccuredExceptionFromSocket += OccuredExceptionClient;
            _sender.DisconnectedSocket += DisconnectedSocket;
        }

        private void InitializeAcceptor()
        {
            _acceptor = new Acceptor(_serverSocket);
            _acceptor.ConnectedClient += ConnectedClient;
            _acceptor.OccuredExceptionFromSocket += OccuredExceptionSocket;
            _acceptor.NewAccept();
        }

        private void ConnectedClient(Socket socket)
        {
            ConnectedSocket(socket);
            _receiver.BeginReceive(socket);
        }

        private void OccuredExceptionSocket(Exception ex)
        {
            if (OccuredError != null)
                OccuredError(ex);
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
            if (_serverSocket != null)
            {
                _serverSocket.Shutdown(SocketShutdown.Both);
                _serverSocket.Dispose();
                _serverSocket = null;
            }
        }

        private IPEndPoint _endPoint;
        private Socket _serverSocket;
        private ManualResetEvent _serverWait = new ManualResetEvent(false);
        private bool _disposed = false;
        private Acceptor _acceptor;
        private Receiver<messageT, headerT, bodyT> _receiver;
        private Sender<messageT, headerT, bodyT> _sender;

        public event ReceiveHandle ReceivedMessageFromClient;
        public delegate void ReceiveHandle(Socket socket, IMessage message);

        public event ErrorHandle OccuredError;
        public delegate void ErrorHandle(Exception ex);

        public event ConnectHandle ConnectedSocket;
        public delegate void ConnectHandle(Socket socket);
    }
}
