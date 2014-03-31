using System;
using Laika.Net.Message;

namespace Laika.Net
{
    public class ReceivedMessageEventArgs : EventArgs
    {
        public ReceivedMessageEventArgs(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; internal set; }
    }
}
