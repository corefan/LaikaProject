using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
namespace Laika.Net
{
    public class Session : IDisposable
    {
        internal class SendContext
        {
            internal int BytesTransferred { get; set; }
            internal byte[] SendData { get; set; }
        }

        public Session() 
        {
            Handle = null;
            ReceiveEventArgs = new SocketAsyncEventArgs();
            SendEventArgs = new SocketAsyncEventArgs();
        }
        ~Session()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool NeedSendingProcedure()
        {
            lock (_lockSend)
            {
                if (_sending == true)
                    return false;

                _sending = true;
                return true;
            }
        }

        public bool EndSend()
        {
            lock (_lockSend)
            {
                if (_sending == true)
                {
                    _sending = false;
                    return true;
                }
                return false;
            }
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
            ReceiveEventArgs.Dispose();
            SendEventArgs.Dispose();
            Handle.Disconnect(false);
            Handle.Close();
            Handle.Dispose();
        }

        public Socket Handle { get; internal set; }
        public long ServerSequenceId { get; internal set; }
        internal SocketAsyncEventArgs ReceiveEventArgs { get; set; }
        internal SocketAsyncEventArgs SendEventArgs { get; set; }
        internal ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();
        internal SendContext Context = new SendContext();
        internal bool SetArgsCompleted { get; set; }

        private bool _disposed = false;
        private bool _sending = false;
        private object _lockSend = new object();
    }
}
