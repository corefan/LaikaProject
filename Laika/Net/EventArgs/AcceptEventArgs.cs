using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Net
{
    public class AcceptEventArgs : EventArgs
    {
        public AcceptEventArgs(Session session)
        {
            SessionHandle = session;
        }
        public Session SessionHandle { get; internal set; }
    }
}
