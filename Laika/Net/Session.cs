using System;
using System.Net.Sockets;
namespace Laika.Net
{
    public class Session : IDisposable
    {
        public Session() 
        {
            Handle = null;
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
        protected virtual void Dispose(bool disposing)
        {
            if (disposed == true)
                return;
            if (disposing == true)
            {
                Clear();
            }
            disposed = true;
        }

        private void Clear()
        {
            Handle.Disconnect(false);
            Handle.Close();
            Handle.Dispose();
        }

        public Socket Handle { get; internal set; }
        public long ServerSequenceId { get; internal set; }
        private bool disposed = false;
    }
}
