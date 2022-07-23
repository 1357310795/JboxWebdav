using NWebDav.Server.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

[assembly: InternalsVisibleTo("NutzCode.Libraries.WebCacheStream")]
namespace NutzCode.Libraries.Web
{
    public class WebStream : Stream, IDisposable
    {
        private static ILogger s_log = LoggerFactory.CreateLogger(typeof(WebStream));
        public CookieCollection Cookies { get; set; }
        public NameValueCollection Headers { get; private set; }
        public string ContentType { get; private set; }
        public string ContentEncoding { get; private set; }
        public long ContentLength { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
        public WebParameters WebParameters { get; set; }
        public bool IsRedirect { get; set; } = false;
        public HttpResponseMessage Response { get; set;}
        public HttpClient Client { get; set; }
        public HttpRequestMessage Request { get; set; }
        public HttpMessageHandler Handler { get; set; }

        private Stream _baseStream;
        private long _position;

        static WebStream()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 48;
        }

        public long FilePosition => _position + WebParameters.RangeStart;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                _baseStream?.Dispose();
                Response?.Dispose();
                Request?.Dispose();
                Handler?.Dispose();
                Client?.Dispose();
                _baseStream = null;
                Response = null;
                Request = null;
                Handler = null;
                Client = null;
            }
        }
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebStream()
        {
            Dispose(false);
        }
        public override void Flush()
        {
            _baseStream?.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_baseStream != null)
                return _baseStream.Seek(offset, origin);
            return 0;
        }

        public override void SetLength(long value)
        {
            _baseStream?.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_baseStream == null)
                return 0;
            int cnt=_baseStream.Read(buffer, offset, count);
            _position += cnt;
            return cnt;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_baseStream == null)
                return 0;
            int cnt= await _baseStream.ReadAsync(buffer, offset, count, cancellationToken);
            _position += cnt;
            //Console.WriteLine($"Raw Stream Position: {_position}");
            return cnt;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream?.Write(buffer, offset, count);
        }

        public override bool CanRead => _baseStream?.CanRead ?? false;

        public override bool CanSeek => _baseStream?.CanSeek ?? false;

        public override bool CanWrite => _baseStream?.CanWrite ?? false;

        public override long Length
        {
            get
            {
                if (_baseStream == null)
                    return 0;
                if (_baseStream.Length == 0)
                    return ContentLength;
                return _baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            { 
                throw new NotSupportedException();
            }
        }


        internal static async Task<string> GetUrlAsync<T,S>(S wb, string postData, string encoding, string uagent = "", Dictionary<string, string> headers = null) where T : WebStream, new() where S : WebParameters
        {
            wb.Encoding = string.IsNullOrEmpty(encoding) ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
            if (!string.IsNullOrEmpty(postData))
                wb.PostData = wb.Encoding.GetBytes(postData);
            if (!string.IsNullOrEmpty(uagent))
                wb.UserAgent = uagent;
            if (headers != null)
            {
                wb.Headers = new NameValueCollection();
                foreach (string n in headers.Keys)
                    wb.Headers.Add(n, headers[n]);
            }
            T wab = await CreateStreamAsync<T,S>(wb);
            return await wab.ToTextAsync();
        }

        internal static async Task<T> CreateStreamAsync<T,S>(S pars, CancellationToken token = new CancellationToken()) where T : WebStream, new() where S : WebParameters
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int numretries = 0;
            do
            {
                T w = await InternalCreateStream(new T(), pars, token);
                bool ret = false;
                if (w.StatusCode != HttpStatusCode.OK && w.StatusCode!=HttpStatusCode.PartialContent)
                    ret = await pars.ProcessError(w);
                if (!pars.SolidRequest && !ret)
                    return w;
                if (!ret)
                {
                    if (((int)w.StatusCode >= 500) || (w.StatusCode == HttpStatusCode.RequestTimeout))
                    {
                        if (sw.ElapsedMilliseconds > pars.SolidRequestTimeoutInMilliseconds)
                            return w;
                        w.Dispose();
                        await BackOff(numretries);
                    }
                    else
                        return w;
                }

                numretries++;
            } while (true);
        }

        private static async Task BackOff(int numRetry)
        {
            Random r = new Random();
            int ms = r.Next(0, (2 ^ Math.Min(numRetry, 8)) * 1000);
            await Task.Delay(ms);
        }

        private static void ParseCookies(WebParameters pars, WebStream wb)
        {
            wb.Headers = new NameValueCollection();
            if (wb.Response.Headers.Contains("Set-Cookie"))
            {
                try
                {
                    IEnumerable<string> sss;
                    wb.Response.Headers.TryGetValues("Set-Cookie", out sss);
                    wb.Cookies = new CookieCollection();
                    if (sss != null)
                    {
                        foreach (string value in sss)
                        {
                            foreach (var singleCookie in value.Split(','))
                            {
                                Match match = Regex.Match(singleCookie, "(.+?)=(.+?);");
                                if (match.Captures.Count == 0)
                                    continue;
                                wb.Cookies.Add(new Cookie(match.Groups[1].ToString(), match.Groups[2].ToString(), "/", wb.Request.RequestUri.Host.Split(':')[0]));
                            }
                        }
                    }
                    if (pars.Cookies != null && pars.Cookies.Count > 0)
                    {
                        foreach (Cookie c in pars.Cookies)
                        {
                            bool found = false;
                            foreach (Cookie d in wb.Cookies)
                            {
                                if (d.Name == c.Name)

                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                wb.Cookies.Add(c);
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            else
            {
                wb.Cookies = pars.Cookies;
            }
           
        }

        private static void ParseHeaders(WebStream wb)
        {
            if ((wb.Response.Headers != null) && (wb.Response.Headers.Any()))
            {
                if (wb.Headers == null)
                    wb.Headers = new NameValueCollection();
                foreach (KeyValuePair<string, IEnumerable<string>> h in wb.Response.Headers)
                {
                    foreach (string r in h.Value)
                    {
                        string val = r;
                        if (val.StartsWith("\"") && val.EndsWith("\""))
                            val = val.Substring(1, val.Length - 2);
                        wb.Headers[h.Key] = val;
                    }
                }
            }
        }

        private static void PopulateCookies(WebParameters pars, WebStream wb, string host)
        {
            if (pars.Cookies != null && pars.Cookies.Count > 0)
            {
                HttpClientHandler cl = pars.GetHttpClientHandler(wb);

                cl.CookieContainer = new CookieContainer();
                foreach (Cookie c in pars.Cookies)
                {
                    if (string.IsNullOrEmpty(c.Domain))
                        c.Domain = host;
                    cl.CookieContainer.Add(c);
                }
            }
        }

        internal static async Task<T> InternalCreateStream<T>(T wb, WebParameters pars, CancellationToken token = new CancellationToken()) where T : WebStream, new()
        {
            try
            {
                wb.WebParameters = pars;
                Uri url = pars.Url;
                Uri referer = pars.Referer;
                Uri bas = new Uri(url.Scheme + "://" + url.Host);
                if ((pars.Method == HttpMethod.Get) && (pars.PostData != null))
                    pars.Method = HttpMethod.Post;
                wb.Request = new HttpRequestMessage(pars.Method, url);
                if (!string.IsNullOrEmpty(pars.UserAgent))
                    wb.Request.Headers.UserAgent.ParseAdd(pars.UserAgent);
                if (pars.PostData != null)
                {
                    wb.Request.Content = new ByteArrayContent(pars.PostData, 0, pars.PostData.Length);
                    wb.Request.Content.Headers.ContentType = new MediaTypeHeaderValue(pars.PostEncoding);
                }
                if (pars.HasRange)
                    wb.Request.Headers.Range = new RangeHeaderValue(pars.RangeStart, pars.RangeEnd);
                if (pars.Headers != null)
                    PopulateHeaders(wb.Request, pars.Headers);
                if (referer != null)
                    wb.Request.Headers.Referrer = referer;
                wb.Handler = pars.GetHttpMessageHandler();
                HttpClientHandler cl = pars.GetHttpClientHandler(wb);
                cl.AllowAutoRedirect = pars.AutoRedirect;
                if (pars.AutoDecompress)
                    cl.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                PopulateCookies(pars, wb, bas.Host);
                if (pars.Proxy != null)
                    cl.Proxy = pars.Proxy;
                wb.Client = new HttpClient(wb.Handler);
                wb.Client.Timeout = TimeSpan.FromMilliseconds(pars.TimeoutInMilliseconds);
                await pars.PostProcessRequest(wb);
                wb.Response = await wb.Client.SendAsync(wb.Request, HttpCompletionOption.ResponseHeadersRead, token);
                s_log.Log(LogLevel.Debug, ()=>$"Created new request: {pars.Url.OriginalString} From {pars.RangeStart} to {pars.RangeEnd}");
                token.ThrowIfCancellationRequested();
                wb._baseStream = await wb.Response.Content.ReadAsStreamAsync();
                wb.ContentType = wb.Response.Content.Headers.ContentType.MediaType;
                wb.ContentEncoding = wb.Response.Content.Headers.ContentEncoding.ToString();
                wb.ContentLength = wb.Response.Content.Headers.ContentLength ?? 0;
                if (wb.ContentLength == 0)
                {
                    //IIS will return ContentLength == 0 when you ask a partial content till the end of file, this will get back our content length
                    if (wb.Response.Content.Headers.ContentRange != null &&
                        wb.Response.Content.Headers.ContentRange.HasRange && wb.Response.Content.Headers.ContentRange.From.HasValue && wb.Response.Content.Headers.ContentRange.To.HasValue)
                    {
                        wb.ContentLength = wb.Response.Content.Headers.ContentRange.To.Value -
                                           wb.Response.Content.Headers.ContentRange.From.Value + 1;
                    }
                }
                ParseCookies(pars, wb);
                ParseHeaders(wb);
                wb.StatusCode = wb.Response.StatusCode;
                return wb;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                wb?.Dispose();
                wb = new T();
                wb.StatusCode = HttpStatusCode.RequestTimeout;
                return wb;
            }
        }
        internal static async Task<WebStream> InternalCreateStream(WebParameters pars, CancellationToken token = new CancellationToken())
        {
            WebStream wb = new WebStream();
            return await InternalCreateStream(wb, pars, token);
        }

        private static void PopulateHeaders(HttpRequestMessage msg, NameValueCollection headers)
        {
            foreach (string s in headers.Keys)
            {
                string k = s.ToLower();
                if (k.StartsWith("content") || k == "expires" || k == "last-modified")
                {
                    if (msg.Content.Headers.Contains(s))
                        msg.Content.Headers.Remove(s);
                    msg.Content.Headers.Add(s, headers[s]);
                }
                else
                {
                    if (msg.Headers.Contains(s))
                        msg.Headers.Remove(s);
                    msg.Headers.Add(s, headers[s]);
                }
            }
        }

        public override void Close()
        {
            try
            {
                _baseStream?.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }

    public static class WebStreamExtensions
    {
        public static string PostFromDictionary(this Dictionary<string, string> dict)
        {
            return String.Join("&",
                dict.Select(a => HttpUtility.UrlEncode(a.Key) + "=" + HttpUtility.UrlEncode(a.Value)));
        }

        public static Dictionary<string, string> ToDictionary(this CookieCollection coll)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (Cookie c in coll)
            {
                data.Add(c.Name, c.Value);
            }
            return data;
        }

        public static string ToText(this WebStream wb)
        {
            Encoding enc = Encoding.UTF8;
            if (!string.IsNullOrEmpty(wb.ContentEncoding))
                enc = Encoding.GetEncoding(wb.ContentEncoding);
            using (StreamReader reader = new StreamReader(wb, enc))
            {
                return reader.ReadToEnd();
            }
        }
        public static async Task<string> ToTextAsync(this WebStream wb)
        {
            Encoding enc = Encoding.UTF8;
            if (!string.IsNullOrEmpty(wb.ContentEncoding))
                enc = Encoding.GetEncoding(wb.ContentEncoding);
            using (StreamReader reader = new StreamReader(wb, enc))
            {
                return await reader.ReadToEndAsync();
            }
        }

    }
}



