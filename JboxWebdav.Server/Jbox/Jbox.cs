using Jbox;
using Jbox.Models;
using Jbox.Service;
using JboxWebdav.Server.Jbox.JboxShared;
using JboxWebdav.Server.Jbox.Upload;
using Newtonsoft.Json;
using NutzCode.Libraries.Web;
using NutzCode.Libraries.Web.StreamProvider;
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
        private static WebDataProvider webDataProvider = new WebDataProvider(20, 4 * 1024, 2, 20000);
        
        public static CommonResult GetDeliveryAuthToken(string deliverycode, string passwordenc)
        {
            //https://jbox.sjtu.edu.cn/v2/delivery/auth/bd8644a5da20485680b29e18e33df40a?account_id=1&uid=388333&S=94F5A20E&_=1658739208000
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = new Dictionary<string, string>();
            forms.Add("password", passwordenc);

            var res = Web.Post($"https://jbox.sjtu.edu.cn/v2/delivery/auth/{deliverycode}", paras, headers, forms, true);

            if (res.code != HttpStatusCode.OK)
                return new CommonResult(false, res.result);
            if (res.result == null)
                return new CommonResult(false, res.message);

            var json = JsonConvert.DeserializeObject<JboxDeliveryAuthDto>(res.result);

            return new CommonResult(true, json.token);
        }

        public static JboxItemInfo GetJboxItemInfo(string path)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = GetCommonBodyForms();

            forms.Add("page_num", "0");
            forms.Add("target_path", path);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/metadata_page/databox",
                paras, headers, forms, true);

            if (!res.success)
                throw new Exception(res.result);
            
            var json = JsonConvert.DeserializeObject<JboxItemInfo>(res.result);

            if (json.success && json.IsDir)
            {
                for (int i = 1; i <= (json.ContentSize - 1) / 50; i++)
                {
                    forms["page_num"] = i.ToString();
                    headers = GetCommonHeaders();
                    var res1 = Web.Post("https://jbox.sjtu.edu.cn/v2/metadata_page/databox", paras, headers, forms, true);
                    if (!res1.success)
                        throw new Exception(res1.result);
                    var json1 = JsonConvert.DeserializeObject<JboxItemInfo>(res1.result);

                    MergePageResults(json, json1);
                }
            }
            return json;
        }

        public static JboxSharedItemInfo GetJboxSharedItemInfo(string delivery_code, string path, string token)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            paras.Add("limit", "100");
            paras.Add("offset", "0");
            paras.Add("token", token ?? "");

            var res = Web.Get($"https://jbox.sjtu.edu.cn/v2/delivery/metadata/{delivery_code}{path}", paras, headers);

            if (!res.success)
                throw new Exception(res.result);

            var json = JsonConvert.DeserializeObject<JboxSharedItemInfo>(res.result);

            if (json.success && json.IsDir)
            {
                for (int i = 1; i <= (json.TotalSize - 1) / 100; i++)
                {
                    paras["offset"] = (i*100).ToString();
                    headers = GetCommonHeaders();
                    var res1 = Web.Get($"https://jbox.sjtu.edu.cn/v2/delivery/metadata/{delivery_code}{path}", paras, headers);
                    if (!res1.success)
                        throw new Exception(res1.result);
                    var json1 = JsonConvert.DeserializeObject<JboxSharedItemInfo>(res1.result);

                    MergePageResults(json, json1);
                }
            }
            json.Token = token;
            return json;
        }

        public static CommonResult UploadToSharedDir(string deliverycode, string path, Stream file, long length)
        {
            var headers = GetCommonHeaders();
            headers.Add("Origin", "https://jbox.sjtu.edu.cn");
            var paras = GetCommonQueryParas();
            paras.Add("utime", Common.GetTimeStampMilli());
            paras.Add("t", Common.GetTimeStampMilli());
            paras.Add("language", "zh");
            paras.Add("source", "file");
            paras.Add("overwrite", "true");
            paras.Add("X-LENOVO-SESS-ID", Jac.finalSESSID);

            string url = $"https://jbox.sjtu.edu.cn:10081/v2/delivery/data/{deliverycode}";
            url += path.UrlEncodeByParts();
            url = Web.BuildUrl(url, paras);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.AllowWriteStreamBuffering = false;
            Web.ProcessHeaders(headers, req);

            #region 处理Form表单请求内容
            string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
            req.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

            //获取请求流
            Stream requestStream = req.GetRequestStream();

            //文件数据模板
            string fileFormHeaderTemplate =
                "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                "\r\nContent-Type: {2}" +
                "\r\n\r\n";

            string formdata = null;
            //上传文件
            formdata = string.Format(
                fileFormHeaderTemplate,
                "file", //表单键
                path.Substring(path.LastIndexOf("/") + 1),
                "application/octet-stream");

            //统一处理
            byte[] formdataBytes = null;

            //第一行不需要换行
            formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));

            requestStream.Write(formdataBytes, 0, formdataBytes.Length);

            //写入文件内容

            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();
            }

            //结尾
            var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(footer, 0, footer.Length);

            #endregion

            requestStream.Close();
            //req.ContentLength = requestStream.Length;

            var res = Web.GetFinalResult(req);
            if (res.code == HttpStatusCode.OK)
                return res;
            else
                return new CommonResult(false, res.message);
        }

        private static void MergePageResults(JboxItemInfo info, JboxItemInfo add)
        {
            JboxItemInfo[] c = new JboxItemInfo[info.Content.Length + add.Content.Length];

            Array.Copy(info.Content, 0, c, 0, info.Content.Length);
            Array.Copy(add.Content, 0, c, info.Content.Length, add.Content.Length);

            info.Content = c;
        }
        private static void MergePageResults(JboxSharedItemInfo info, JboxSharedItemInfo add)
        {
            JboxSharedItemInfo[] c = new JboxSharedItemInfo[info.Content.Length + add.Content.Length];

            Array.Copy(info.Content, 0, c, 0, info.Content.Length);
            Array.Copy(add.Content, 0, c, info.Content.Length, add.Content.Length);

            info.Content = c;
        }

        public static Stream GetFile(string path, long length, long? start = null, long? end = null)
        {
            ParameterResolverProvider parameterResolverProvider = new ParameterResolverProvider(path, length);

            SeekableWebStream stream = new SeekableWebStream(path, start ?? 0, end ?? length, webDataProvider, parameterResolverProvider.ParameterResolver);

            return stream;
        }

        public static Stream GetSharedFile(JboxSharedItemInfo item, long? start = null, long? end = null)
        {
            ParameterResolverProvider_Shared parameterResolverProvider = new ParameterResolverProvider_Shared(item.DownloadUrl, item.Bytes, item.Token);

            SeekableWebStream stream = new SeekableWebStream(item.DownloadUrl, start ?? 0, end ?? item.Bytes, webDataProvider, parameterResolverProvider.ParameterResolver);

            return stream;
        }

        public static JboxCreateDirInfo CreateDirectory(string path)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = GetCommonBodyForms1(path);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/fileops/create_folder/databox",
                paras, headers, forms, true);

            if (res.result == null)
                throw new Exception(res.message);

            var json = JsonConvert.DeserializeObject<JboxCreateDirInfo>(res.result);

            return json;
        }

        public static CommonResult UploadFile(string path, Stream file, long length)
        {
            Uploader u = new Uploader(path, file, length);
            return u.Run();
        }

        public static JboxMoveItemInfo MoveJboxItem(string source, string destDir)
        {
            string reqBodyValue = "{\"to\":{\"root\":\"databox\",\"path\":\"" + destDir + "\",\"path_type\":\"self\",\"from\":\"\"},\"from\":[{\"root\":\"databox\",\"path\":\"" + source + "\",\"path_type\":\"self\",\"from\":\"\"}],\"other_data\":\"\"}";

            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = new Dictionary<string, string>();
            forms.Add("json", reqBodyValue);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/fileops/batch_move", paras, headers, forms, true);

            if (res.result == null)
                throw new Exception(res.message);

            var json = JsonConvert.DeserializeObject<JboxMoveItemInfo>(res.result);

            return json;
        }

        public static JboxMoveItemInfo RenameJboxItem(string source, string dest)
        {
            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = new Dictionary<string, string>();
            forms.Add("root", "databox");
            forms.Add("from_path_type", "self");
            forms.Add("to_path_type", "self");
            forms.Add("from_path", source);
            forms.Add("to_path", dest);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/fileops/move", paras, headers, forms, true);

            if (res.result == null)
                throw new Exception(res.message);

            var json = JsonConvert.DeserializeObject<JboxMoveItemInfo>(res.result);

            return json;
        }

        public static JboxMoveItemInfo DeleteJboxItem(string path)
        {
            string reqBodyValue = "{\"pathes\":[{\"root\":\"databox\",\"path\":\"" + path + "\",\"path_type\":\"self\",\"from\":\"\"}],\"ignore_share\":true}";

            var headers = GetCommonHeaders();
            var paras = GetCommonQueryParas();
            var forms = new Dictionary<string, string>();
            forms.Add("json", reqBodyValue);

            var res = Web.Post("https://jbox.sjtu.edu.cn/v2/fileops/batch_delete", paras, headers, forms, true);

            if (res.result == null)
                throw new Exception(res.message);

            var json = JsonConvert.DeserializeObject<JboxMoveItemInfo>(res.result);

            return json;
        }

        public static JboxMoveItemInfo DeleteJboxItem(JboxItemInfo item)
        {
            return DeleteJboxItem(item.Path);
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
            d.Add("page_button_count", "9999999");
            d.Add("page_size", "50");
            d.Add("include_deleted", "false");
            return d;
        }

        public static Dictionary<string, string> GetCommonBodyForms1(string path)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("path_type", "self");
            d.Add("is_update", "false");
            d.Add("targetPath", path);
            return d;
        }
    }
}
