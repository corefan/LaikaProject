using System;

namespace Laika.Web
{
    public class AsyncResponseEventArgs : EventArgs
    {
        public AsyncResponseEventArgs(string responseData)
        {
            ResponseData = responseData;
        }

        public string ResponseData { get; private set; }
    }
}
