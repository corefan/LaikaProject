using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
