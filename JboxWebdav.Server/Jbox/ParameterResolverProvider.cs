using Jbox.Service;
using NutzCode.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    public class ParameterResolverProvider
    {
        public ParameterResolverProvider(string path, long length)
        {
            this.path = path;
            this.length = length;
        }

        public string path { get; set; }
        public long length { get; set; }

        public SeekableWebParameters ParameterResolver(long start)
        {
            Dictionary<string, string> q = new Dictionary<string, string>();
            q.Add("path_type", "self");
            q.Add("S", Jac.finalS);

            string url = "https://jbox.sjtu.edu.cn:10081/v2/files/databox";
            url += path.UrlEncodeByParts();
            StringBuilder builder1 = new StringBuilder();
            builder1.Append(url);

            if (q.Count > 0)
            {
                builder1.Append("?");
                int i = 0;
                foreach (var item in q)
                {
                    if (i > 0)
                        builder1.Append("&");
                    builder1.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            SeekableWebParameters para = new SeekableWebParameters(new Uri(builder1.ToString()), path, 1024 * 1024);
            para.TimeoutInMilliseconds = 30 * 1000;
            para.Cookies = new System.Net.CookieCollection();
            para.Cookies.Add(new System.Net.Cookie("S", Jac.finalS, "/", "jbox.sjtu.edu.cn"));
            para.Cookies.Add(new System.Net.Cookie("X-LENOVO-SESS-ID", Jac.finalSESSID, "/", "jbox.sjtu.edu.cn"));
            para.HasRange = true;
            para.Method = HttpMethod.Get;
            para.Headers = new System.Collections.Specialized.NameValueCollection();
            para.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            //para.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            para.Headers.Add("Referer", "https://jbox.sjtu.edu.cn/");
            para.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            para.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");

            para.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/84.0.1312.57 Safari/537.17";
            para.Referer = new Uri("https://jbox.sjtu.edu.cn/");

            para.HasRange = true;
            para.RangeStart = start;
            para.RangeEnd = length - 1;
            return para;
        }
    }

    public class ParameterResolverProvider_Shared
    {
        public ParameterResolverProvider_Shared(string path, long length, string token)
        {
            this.path = path;
            this.length = length;
            this.token = token;
        }

        public string path { get; set; }
        public long length { get; set; }
        public string token { get; set; }

        public SeekableWebParameters ParameterResolver(long start)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("token", token ?? "");

            StringBuilder builder1 = new StringBuilder();
            builder1.Append(path);

            if (query.Count > 0)
            {
                builder1.Append("?");
                int i = 0;
                foreach (var item in query)
                {
                    if (i > 0)
                        builder1.Append("&");
                    builder1.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            SeekableWebParameters para = new SeekableWebParameters(new Uri(builder1.ToString()), path, 1024 * 1024);
            para.TimeoutInMilliseconds = 30 * 1000;
            para.Cookies = new System.Net.CookieCollection();
            para.Cookies.Add(new System.Net.Cookie("S", Jac.finalS, "/", "jbox.sjtu.edu.cn"));
            para.Cookies.Add(new System.Net.Cookie("X-LENOVO-SESS-ID", Jac.finalSESSID, "/", "jbox.sjtu.edu.cn"));
            para.HasRange = true;
            para.Method = HttpMethod.Get;
            para.Headers = new System.Collections.Specialized.NameValueCollection();
            para.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            //para.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            para.Headers.Add("Referer", "https://jbox.sjtu.edu.cn/");
            para.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            para.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");

            para.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/84.0.1312.57 Safari/537.17";
            para.Referer = new Uri("https://jbox.sjtu.edu.cn/");

            para.HasRange = true;
            para.RangeStart = start;
            para.RangeEnd = length - 1;
            return para;
        }
    }

    public class ParameterResolverProvider_Public
    {
        public ParameterResolverProvider_Public(string path, long length)
        {
            this.path = path;
            this.length = length;
        }

        public string path { get; set; }
        public long length { get; set; }

        public SeekableWebParameters ParameterResolver(long start)
        {
            Dictionary<string, string> q = new Dictionary<string, string>();
            q.Add("path_type", "ent");
            q.Add("S", Jac.finalS);

            string url = "https://jbox.sjtu.edu.cn:10081/v2/files/databox";
            url += path.UrlEncodeByParts();
            StringBuilder builder1 = new StringBuilder();
            builder1.Append(url);

            if (q.Count > 0)
            {
                builder1.Append("?");
                int i = 0;
                foreach (var item in q)
                {
                    if (i > 0)
                        builder1.Append("&");
                    builder1.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            SeekableWebParameters para = new SeekableWebParameters(new Uri(builder1.ToString()), path, 1024 * 1024);
            para.TimeoutInMilliseconds = 30 * 1000;
            para.Cookies = new System.Net.CookieCollection();
            para.Cookies.Add(new System.Net.Cookie("S", Jac.finalS, "/", "jbox.sjtu.edu.cn"));
            para.Cookies.Add(new System.Net.Cookie("X-LENOVO-SESS-ID", Jac.finalSESSID, "/", "jbox.sjtu.edu.cn"));
            para.HasRange = true;
            para.Method = HttpMethod.Get;
            para.Headers = new System.Collections.Specialized.NameValueCollection();
            para.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            //para.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            para.Headers.Add("Referer", "https://jbox.sjtu.edu.cn/");
            para.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            para.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");

            para.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/84.0.1312.57 Safari/537.17";
            para.Referer = new Uri("https://jbox.sjtu.edu.cn/");

            para.HasRange = true;
            para.RangeStart = start;
            para.RangeEnd = length - 1;
            return para;
        }
    }
}
