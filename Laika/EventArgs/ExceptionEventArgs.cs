using System;

namespace Laika.Net
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Session session, Exception ex)
        {
            Session = session;
            Exception = ex;
        }
        public Session Session { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
