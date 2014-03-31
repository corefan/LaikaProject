using System;

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
