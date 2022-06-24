using Jbox.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Jbox.Models.WebResult;

namespace Jbox.Service
{
    public static class Web
    {
        public static CommonResult Post(string url, Dictionary<string, string> headers, Dictionary<string, string> formdata, bool urlencode)
        {
            #region 构造表单数据
            StringBuilder builder = new StringBuilder();
            if (formdata.Count > 0)
            {
                int i = 0;
                foreach (var item in formdata)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            #endregion

            #region 初始化请求
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            if (headers.ContainsKey("Accept"))
            {
                req.Accept = headers["Accept"];
                headers.Remove("Accept");
            }
            if (headers.ContainsKey("UserAgent"))
            {
                req.Accept = headers["UserAgent"];
                headers.Remove("UserAgent");
            }
            if (headers.ContainsKey("Referer"))
            {
                req.Accept = headers["Referer"];
                headers.Remove("Referer");
            }
            if (headers.ContainsKey("Connection") && headers["Connection"] == "keep-alive")
            {
                req.KeepAlive = true;
                headers.Remove("Connection");
            }
            if (headers.ContainsKey("Origin"))
            {
                req.Accept = headers["Origin"];
                headers.Remove("Origin");
            }
            if (headers.ContainsKey("Host"))
            {
                headers.Remove("Host");
            }
            if (headers.ContainsKey("Content-Length"))
            {
                headers.Remove("Content-Length");
            }
            if (headers.ContainsKey("Cache-Control"))
            {
                headers.Remove("Cache-Control");
            }
            foreach (var i in headers)
            {
                req.Headers[i.Key] = i.Value;
            }
            #endregion

            #region 添加Post 参数

            byte[] data = urlencode ? System.Web.HttpUtility.UrlEncodeToBytes(builder.ToString()) : Encoding.Default.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                return GetResponseBody(resp, stream);
            }
            catch (Exception ex)
            {
                return new CommonResult(false, ex.ToString());
            }
        }

        public static CommonResult Post(string url, Dictionary<string, string> headers, string formdata, bool urlencode)
        {
            #region 初始化请求
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            if (headers.ContainsKey("Accept"))
            {
                req.Accept = headers["Accept"];
                headers.Remove("Accept");
            }
            if (headers.ContainsKey("User-Agent"))
            {
                req.UserAgent = headers["User-Agent"];
                headers.Remove("User-Agent");
            }
            if (headers.ContainsKey("Referer"))
            {
                req.Referer = headers["Referer"];
                headers.Remove("Referer");
            }
            if (headers.ContainsKey("Connection") && headers["Connection"] == "keep-alive")
            {
                req.KeepAlive = true;
                headers.Remove("Connection");
            }
            if (headers.ContainsKey("Content-Type"))
            {
                req.ContentType = headers["Content-Type"];
                headers.Remove("Content-Type");
            }
            if (headers.ContainsKey("Host"))
            {
                headers.Remove("Host");
            }
            if (headers.ContainsKey("Content-Length"))
            {
                headers.Remove("Content-Length");
            }
            if (headers.ContainsKey("Cache-Control"))
            {
                headers.Remove("Cache-Control");
            }
            foreach (var i in headers)
            {
                req.Headers[i.Key] = i.Value;
            }
            #endregion

            #region 添加Post 参数

            byte[] data = urlencode ? System.Web.HttpUtility.UrlEncodeToBytes(formdata) : Encoding.Default.GetBytes(formdata);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            //获取响应
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                return GetResponseBody(resp, stream);
            }
            catch (Exception ex)
            {
                return new CommonResult(false, ex.ToString());
            }
        }

        public static CommonResult Get(string url, Dictionary<string, string> headers)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            if (headers.ContainsKey("Accept"))
            {
                req.Accept = headers["Accept"];
                headers.Remove("Accept");
            }
            if (headers.ContainsKey("User-Agent"))
            {
                req.UserAgent = headers["User-Agent"];
                headers.Remove("User-Agent");
            }
            if (headers.ContainsKey("Referer"))
            {
                req.Referer = headers["Referer"];
                headers.Remove("Referer");
            }
            if (headers.ContainsKey("Connection") && headers["Connection"] == "keep-alive")
            {
                req.KeepAlive = true;
                headers.Remove("Connection");
            }
            if (headers.ContainsKey("Content-Type"))
            {
                req.ContentType = headers["Content-Type"];
                headers.Remove("Content-Type");
            }
            if (headers.ContainsKey("Host"))
            {
                headers.Remove("Host");
            }
            if (headers.ContainsKey("Content-Length"))
            {
                headers.Remove("Content-Length");
            }
            if (headers.ContainsKey("Cache-Control"))
            {
                headers.Remove("Cache-Control");
            }
            foreach (var i in headers)
            {
                req.Headers[i.Key] = i.Value;
            }

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                return GetResponseBody(resp, stream);
            }
            catch (Exception ex)
            {
                return new CommonResult(false, ex.ToString());
            }
        }

        public static CommonResult GetResponseBody(HttpWebResponse resp, Stream stream)
        {
            string result;
            //获取响应内容
            if (resp.ContentEncoding != null && resp.ContentEncoding.ToLower() == "gzip") 
            {
                GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
                //对解压缩后的字符串信息解析
                using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))//中文编码处理
                {
                    result = reader.ReadToEnd();
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            return new CommonResult(true, result);
        }

        public static Dictionary<string, string> PostCommonHeaders()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            d.Add("Accept-Encoding", "gzip, deflate, br");
            d.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            d.Add("Cache-Control", "no-cache");
            d.Add("Connection", "keep-alive");
            d.Add("Content-Length", "429");
            d.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            d.Add("Cookie", Jac.mycookie);
            d.Add("Pragma", "no-cache");
            d.Add("sec-ch-ua", "\"Chromium\";v=\"94\", \"Microsoft Edge\";v=\"94\", \"; Not A Brand\";v=\"99\"");
            d.Add("sec-ch-ua-mobile", "?0");
            d.Add("sec-ch-ua-platform", "\"Windows\"");
            d.Add("Sec-Fetch-Dest", "empty");
            d.Add("Sec-Fetch-Mode", "cors");
            d.Add("Sec-Fetch-Site", "same-origin");
            d.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.9 Safari/537.36");
            d.Add("X-Requested-With", "XMLHttpRequest");
            return d;
        }
    }
}
