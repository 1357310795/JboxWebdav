using Jbox;
using Jbox.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    public static class JboxService
    {

        public static JboxDirectoryInfo GetJboxDirectoryInfo(string path)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = GetCommonBodyForms();

            forms.Add("target_path", path);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/metadata_page/databox",
                paras, headers, forms, true);

            if (!res.success)
                throw new Exception(res.result);

            var json = JsonConvert.DeserializeObject<JboxDirectoryInfo>(res.result);
            return json;
        }

        public static JboxFileInfo GetJboxFileInfo(string path)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = GetCommonBodyForms();

            forms.Add("target_path", path);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/metadata_page/databox",
                paras, headers, forms, true);

            if (!res.success)
                throw new Exception(res.result);

            var json = JsonConvert.DeserializeObject<JboxFileInfo>(res.result);
            return json;
        }

        public static JboxItemInfo GetJboxItemInfo(string path)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = GetCommonBodyForms();

            forms.Add("target_path", path);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/metadata_page/databox",
                paras, headers, forms, true);

            if (!res.success)
                throw new Exception(res.result);
            
            var json = JsonConvert.DeserializeObject<JboxItemInfo>(res.result);

            return json;
        }

        public static Stream GetFile(string path, long? start=null, long? end=null)
        {
            Dictionary<string, string> h = new Dictionary<string, string>();
            h.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            h.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/84.0.1312.57 Safari/537.17");
            h.Add("Content-Type", "application/x-www-form-urlencoded");
            h.Add("Referer", "https://jbox.sjtu.edu.cn/");
            h.Add("Accept-Encoding", "gzip, deflate, br");
            h.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            h.Add("Cookie", Jac.mycookie);
            if (start != null && end != null)
            {
                //x-lenovows-range: bytes=56623104-75497472
                h.Add("Content-Range", $"bytes={start}-{end}");
                h.Add("x-lenovows-range", $"bytes={start}-{end}");
            }
            

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

            #region 初始化请求
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder1.ToString());
            req.Method = "GET";
            Web.ProcessHeaders(h, req);
            #endregion

            #region 获取响应
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                return stream;
            }
            catch (Exception ex)
            {
                return Stream.Null;
            }
            #endregion
        }

        public static Dictionary<string, string> GetCommonHeaders()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Accept", "application/json, text/plain, */*");
            d.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/84.0.1312.57 Safari/537.17");
            d.Add("Content-Type", "application/x-www-form-urlencoded");
            d.Add("Referer", "https://jbox.sjtu.edu.cn/v/list/self");
            d.Add("Accept-Encoding", "gzip, deflate, br");
            d.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            d.Add("Cookie", Jac.mycookie);
            return d;
        }

        public static Dictionary<string, string> GetCommonQueryParas()
        {
            //https://jbox.sjtu.edu.cn/v2/metadata_page/databox?account_id=1&uid=388333&S=32116B31&_=1656074494000
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("account_id", Jac.userInfo.AccountId.ToString());
            d.Add("uid", Jac.userInfo.Uid.ToString());
            d.Add("S", Jac.finalS);
            d.Add("_", Common.GetTimeStampMilli());
            return d;
        }

        public static Dictionary<string, string> GetCommonBodyForms()
        {
            //path_type=self&sort=asc&orderby=name&page_button_count=5&page_size=100&page_num=0&include_deleted=false&neid=1494956305029279781&nsid=388579&target_path=%2F%F0%9F%98%85&prefix_neid=
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("path_type", "self");
            d.Add("sort", "asc");
            d.Add("orderby", "name");
            d.Add("page_button_count", "5");
            d.Add("page_num", "0");
            d.Add("page_size", "50");
            d.Add("include_deleted", "false");
            return d;
        }
    }
}
