using System;

namespace Laika.Net
{
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }
        public Exception Exception { get; internal set; }
    }
}
