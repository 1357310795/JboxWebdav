using Jbox;
using Jbox.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox.JboxShared
{
    public static class JboxShared
    {
        public static List<JboxSharedModel> items;
        public static List<JboxSharedModel> Get()
        {
            if (items != null) return items;
            var filepath = Path.Combine(Config.AppDataDir, "JboxSharedModels.json");
            if (File.Exists(filepath))
            {
                items = JsonConvert.DeserializeObject<List<JboxSharedModel>>(File.ReadAllText(filepath));
            }
            else
            {
                items = new List<JboxSharedModel>();
            }
            return items;
        }

        public static void Save()
        {
            var filepath = Path.Combine(Config.AppDataDir, "JboxSharedModels.json");
            Directory.CreateDirectory(Config.AppDataDir);
            File.WriteAllText(filepath, JsonConvert.SerializeObject(items));
        }

        public static bool CheckIsValid(JboxSharedModel model)
        {
            //Full URL
            var regex1 = new Regex("^(https?)?jbox.sjtu.edu.cnvlinkview([a-z0-9]{32})$");
            var regex2 = new Regex("^([a-z0-9]{32})$");
            var match1 = regex1.Match(model.Name);
            if (match1.Success)
            {
                model.DeliveryCode = match1.Groups[2].Value;
                model.State = JboxSharedState.ok;
                return true;
            }
            var match2 = regex2.Match(model.Name);
            if (match2.Success)
            {
                model.DeliveryCode = match2.Groups[1].Value;
                model.State = JboxSharedState.ok;
                return true;
            }
            //Short URL With Password
            string shorturl = "";
            var regex233 = new Regex("^(https?)?jbox.sjtu.edu.cnl([a-z0-9A-Z]{6}).+?提取码：([a-z0-9A-Z]{1,6})");
            var match233 = regex233.Match(model.Name);
            if (match233.Success)
            {
                shorturl = match233.Groups[2].Value;
            }
            //Short URL
            else
            {
                var regex3 = new Regex("^(https?)?jbox.sjtu.edu.cnl([a-z0-9A-Z]{6})$");
                var match3 = regex3.Match(model.Name);
                if (match3.Success)
                {
                    shorturl = match3.Groups[2].Value;
                }
                else
                {
                    var regex4 = new Regex("^([a-z0-9A-Z]{6})$");
                    var match4 = regex4.Match(model.Name);
                    if (match4.Success)
                    {
                        shorturl = match4.Groups[1].Value;
                    }
                }
                if (shorturl == "")
                {
                    model.State = JboxSharedState.invalid;
                    return false;
                }
            }
            
            shorturl = $"https://jbox.sjtu.edu.cn/l/{shorturl}";
            HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(shorturl);
            req1.AllowAutoRedirect = false;

            HttpWebResponse resp1 = (HttpWebResponse)req1.GetResponse();

            if (resp1.StatusCode != HttpStatusCode.Redirect)
            {
                model.State = JboxSharedState.error;
                return false;
            }

            string fullurl = resp1.Headers["Location"];
            var regex5 = new Regex("^https://jbox.sjtu.edu.cn/v/link/view/([a-z0-9]{32})$");
            var match5 = regex5.Match(fullurl);
            if (match5.Success)
            {
                model.DeliveryCode = match5.Groups[1].Value;
                model.State = JboxSharedState.ok;
                if (match233.Success)
                {
                    var password = match233.Groups[3].Value;
                    var password_enc = Common.RSAEncrypt(Jac.publicKey, password);
                    var tokenres = JboxService.GetDeliveryAuthToken(model.DeliveryCode, password_enc);
                    if (tokenres.success)
                    {
                        model.Token = tokenres.result;
                        model.State = JboxSharedState.ok;
                        JboxShared.Save();
                        return true;
                    }
                    else
                    {
                        model.State = JboxSharedState.needpassword;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                model.State = JboxSharedState.error;
                return false;
            }
        }
    }

    public class JboxSharedModel
    {
        public string DeliveryCode { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public JboxSharedState State { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
        public string Token { get; set; }
        public string AltName { get; set; }
        public long CreatorUid { get; set; }
    }

    public enum JboxSharedState
    {
        invalid, needpassword, expired, ok, error
    }
}
