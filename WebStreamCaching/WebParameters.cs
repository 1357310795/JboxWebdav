using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NutzCode.Libraries.Web
{
    public class WebParameters
    {


        public Uri Url { get; set; }
        public byte[] PostData { get; set; } = null;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public string PostEncoding { get; set; } = "application/x-www-form-urlencoded";
        public string UserAgent { get; set; } = null;
        public CookieCollection Cookies { get; set; } = null;
        public NameValueCollection Headers { get; set; } = null;
        public int TimeoutInMilliseconds { get; set; } = 10000;
        public Uri Referer { get; set; } = null;
        public IWebProxy Proxy { get; set; } = null;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Func<WebStream, object, Task> RequestCallback { get; set; } = null;
        public Func<WebStream, object, Task<bool>> ErrorCallback { get; set; } = null; 
        public bool AutoRedirect { get; set; } = true;
        public bool AutoDecompress { get; set; } = true;
        public bool SolidRequest { get; set; } = true;
        public int SolidRequestTimeoutInMilliseconds { get; set; } = 10*60*1000;
        public bool HasRange { get; set; } = false;
        public long RangeStart { get; set; } = 0;
        public long RangeEnd { get; set; } = long.MaxValue;

        public object RequestCallbackParameter { get; set; } = null;
        public object ErrorCallbackParameter { get; set; } = null;

        public WebParameters(Uri url)
        {
            Url = url;
        }

        public virtual WebParameters Clone()
        {
            WebParameters n=new WebParameters(Url);
            this.CopyTo(n);
            return n;
        }

        public virtual HttpMessageHandler GetHttpMessageHandler()
        {
            return new HttpClientHandler();
        }

        public virtual HttpClientHandler GetHttpClientHandler(WebStream ws)
        {
            return (HttpClientHandler)ws.Handler;
        }
        public async Task PostProcessRequest(WebStream w)
        {
           if (RequestCallback!=null)
                await RequestCallback(w,RequestCallbackParameter);
        }
        public async Task<bool> ProcessError(WebStream w)
        {
            if (ErrorCallback != null)
                return await ErrorCallback(w,ErrorCallbackParameter);
            return false;
        }

    }
}
