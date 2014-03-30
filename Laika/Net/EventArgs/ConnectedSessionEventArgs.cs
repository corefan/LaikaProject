using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Net
{
    public class ConnectedSessionEventArgs : EventArgs
    {
        public ConnectedSessionEventArgs(Session session)
        {
            SessionHandle = session;
        }
        public Session SessionHandle { get; internal set; }
    }
}
