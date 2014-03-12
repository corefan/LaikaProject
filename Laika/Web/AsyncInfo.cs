using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Laika.Web
{
    internal class AsyncInfo : IDisposable
    {
        internal AsyncInfo(WebRequest request, Stream dataStream, WebResponse response = null)
        {
            DataRequest = request;
            DataStream = dataStream;
            DataResponse = response;
            DataRead = new StringBuilder();
            Buffer = new byte[_defaultBufferSize];
        }

        ~AsyncInfo()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string GetReadString()
        {
            return DataRead.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;

            if (disposing == true)
            {
                DataStream.Dispose();
                DataResponse.Close();
            }

            _disposed = true;
        }

        internal WebRequest DataRequest { get; set; }
        internal WebResponse DataResponse { get; set; }
        internal Stream DataStream { get; set; }
        internal StringBuilder DataRead { get; set; }
        internal byte[] Buffer { get; set; }

        private const int _defaultBufferSize = 1024;
        private bool _disposed = false;
    }
}
