using System;

namespace Laika.Net
{
    public class ExceptionFromSessionEventArgs : EventArgs
    {
        public ExceptionFromSessionEventArgs(Session session, Exception ex)
        { 
            SessionHandle = session;
            Exception = ex;
        }
        public Session SessionHandle { get; private set; }
        public Exception Exception { get; private set; }
    }
}
