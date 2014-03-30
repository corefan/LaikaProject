using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Net
{
    public class DisconnectSocketEventArgs : EventArgs
    {
        public DisconnectSocketEventArgs(Session session)
        {
            SessionHandle = session;
        }
        public Session SessionHandle { get; private set; }
    }
}
