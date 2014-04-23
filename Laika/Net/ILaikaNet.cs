using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laika.Net.Message;
using Laika.Net.Header;
using Laika.Net.Body;

namespace Laika.Net
{
    public interface ILaikaNet
    {
        IMessage MessageFactory();
    }
}
