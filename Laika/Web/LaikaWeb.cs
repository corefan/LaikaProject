using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Laika.Web
{
    /// <summary>
    /// Web request 대응 class, REST API 요청 대응 가능.
    /// </summary>
    public class LaikaWeb
    {
        /// <summary>
        /// post data string factory
        /// </summary>
        /// <param name="postData">post data in dictionary</param>
        /// <returns>post data string</returns>
        public static string MakePostDataString(Dictionary<string, string> postData)
        {
            if (postData == null || postData.Count <= 0)
                return null;

            List<string> posts = new List<string>();

            foreach (KeyValuePair<string, string> pair in postData)
            {
                posts.Add(pair.Key + "=" + pair.Value);
            }

            return string.Join("&", posts);
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="url">요청할 URL</param>
        public LaikaWeb(string url)
        {
            Url = url;
        }
        /// <summary>
        /// 일반 요청 비동기 메소드
        /// </summary>
        /// <param name="type">content type</param>
        /// <param name="method">request method</param>
        public void RequestAsync(ContentType type, string method = "GET", int timeoutMS = 100000)
        {
            WebRequest request = GetRequest(type, method, timeoutMS);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();

            AsyncInfo info = new AsyncInfo(request, stream, response);
            info.DataStream.BeginRead(info.Buffer, 0, info.Buffer.Length, new AsyncCallback(ResponseCallback), info);
        }
        /// <summary>
        /// 일반 요청 메소드
        /// </summary>
        /// <param name="type">content type</param>
        /// <param name="method">request method</param>
        /// <returns></returns>
        public string Request(ContentType type, string method = "GET", int timeoutMS = 100000)
        {
            WebRequest request = GetRequest(type, method, timeoutMS);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            
            string responseData = reader.ReadToEnd();
            reader.Dispose();
            stream.Dispose();
            return responseData;
        }
        /// <summary>
        /// Post data와 함께 request하는 비동기 메소드
        /// </summary>
        /// <param name="postData">post data</param>
        /// <param name="type">content type</param>
        /// <param name="method">request method</param>
        public void RequestWithPostDataAsync(string postData, ContentType type, string method = "POST", int timeoutMS = 100000)
        {
            if (string.IsNullOrEmpty(postData))
                throw new ArgumentNullException("Invalid post data.");

            byte[] postByteData = Encoding.UTF8.GetBytes(postData);

            WebRequest request = GetRequest(type, method, timeoutMS);
            request.ContentLength = postByteData.Length;

            Stream stream = request.GetRequestStream();
            stream.BeginWrite(postByteData, 0, postByteData.Length, new AsyncCallback(RequestCallback), new AsyncInfo(request, stream));
        }
        /// <summary>
        /// Post data와 함께 request 하는 메소드
        /// </summary>
        /// <param name="postData">post data</param>
        /// <param name="type">content type</param>
        /// <param name="method">request method</param>
        /// <returns></returns>
        public string RequestWithPostData(string postData, ContentType type, string method = "POST", int timeoutMS = 100000)
        {
            if (string.IsNullOrEmpty(postData))
                throw new ArgumentNullException("Invalid post data.");

            byte[] postByteData = Encoding.UTF8.GetBytes(postData);

            WebRequest request = GetRequest(type, method, timeoutMS);
            request.ContentLength = postByteData.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(postByteData, 0, postByteData.Length);
            stream.Close();

            WebResponse response = request.GetResponse();
            stream = response.GetResponseStream();

            StreamReader reader = new StreamReader(stream);
            string responseData = reader.ReadToEnd();

            reader.Dispose();
            stream.Dispose();
            response.Close();

            return responseData;
        }

        private void RequestCallback(IAsyncResult ar)
        {
            AsyncInfo info = ar.AsyncState as AsyncInfo;
            info.DataStream.EndWrite(ar);
            info.DataStream.Close();

            info.DataResponse = info.DataRequest.GetResponse();
            info.DataStream = info.DataResponse.GetResponseStream();
            info.DataStream.BeginRead(info.Buffer, 0, info.Buffer.Length, new AsyncCallback(ResponseCallback), info);
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            AsyncInfo info = ar.AsyncState as AsyncInfo;
            int readByte = info.DataStream.EndRead(ar);
            string readData = null;
            if (readByte > 0)
            {
                info.DataRead.Append(Encoding.UTF8.GetString(info.Buffer, 0, readByte));
                info.DataStream.BeginRead(info.Buffer, 0, info.Buffer.Length, new AsyncCallback(ResponseCallback), info);
            }
            else
            {
                readData = info.GetReadString();
                info.Dispose();
                if (RequestResult != null)
                    RequestResult(this, new AsyncResponseEventArgs(readData));
            }
        }

        private string GetContentType(ContentType type)
        {
            switch(type)
            {
                case ContentType.Application_xml:
                    return "Application/xml";

                case ContentType.Application_javascript:
                    return "Application/javascript";

                case ContentType.Application_octet_stream:
                    return "Application/octet-stream";

                case ContentType.Application_ogg:
                    return "Application/ogg";

                case ContentType.Application_x_shockwave_flash:
                    return "Application/x-shockwave-flash";

                case ContentType.Application_json:
                    return "Application/json";

                case ContentType.Application_x_www_form_urlencode:
                    return "Application/x-www-form-urlencode";

                case ContentType.text_css:
                    return "text/css";

                case ContentType.text_html:
                    return "text/html";

                case ContentType.text_javascript:
                    return "text/javascript";

                case ContentType.text_plain:
                    return "text/plain";

                case ContentType.text_xml:
                    return "text/xml";

                default:
                    throw new ArgumentException("Invalid Content type.");
            }
        }

        private WebRequest GetRequest(ContentType type, string method, int timeoutMS)
        {
            WebRequest request = WebRequest.Create(Url);
            request.Method = method;
            request.Proxy = null;
            request.ContentType = GetContentType(type);
            request.Timeout = timeoutMS;
            return request;
        }
        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 비동기 처리 시 response data 를 받을 때 발생하는 event
        /// </summary>
        public event ResponseEventHandler RequestResult;
        public delegate void ResponseEventHandler(object sender, AsyncResponseEventArgs e);
    }
}
