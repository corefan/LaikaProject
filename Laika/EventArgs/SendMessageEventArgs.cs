using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.Net;

namespace Laika.Net
{
    public class SendMessageEventArgs : EventArgs
    {
        public SendMessageEventArgs(Session session)
        {
            SessionHandle = session;
        }

        public Session SessionHandle { get; private set; }
    }
}
