using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace Laika.Net
{
    public class Session
    {
        public Socket Handle { get; internal set; }
    }
}
