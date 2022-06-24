using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoQiangke.Helpers
{
    public class CookieHelper
    {
        /// <summary>
        /// 解析Cookie
        /// </summary>
        private static readonly Regex RegexSplitCookie2 = new Regex("[^,][\\S\\s]+?;+[\\S\\s]+?(?=,\\S)");

        /// <summary>
        /// 获取所有Cookie 通过Set-Cookie
        /// </summary>
        /// <param name="setCookie"></param>
        /// <returns></returns>
        public static CookieCollection GetCookiesByHeader(string setCookie)
        {
            Debug.WriteLine(setCookie);
            var cookieCollection = new CookieCollection();
            //拆分Cookie
            //var listStr = RegexSplitCookie.Split(setCookie);
            setCookie += ",T";//配合RegexSplitCookie2 加入后缀
            var listStr = RegexSplitCookie2.Matches(setCookie);
            //循环遍历
            foreach (Match item in listStr)
            {
                //根据; 拆分Cookie 内容
                var cookieItem = item.Value.Split(';');
                var cookie = new Cookie();
                cookie.Domain = "jaccount.sjtu.edu.cn";
                for (var index = 0; index < cookieItem.Length; index++)
                {
                    var info = cookieItem[index];
                    //第一个 默认 Cookie Name
                    //判断键值对
                    if (info.Contains("="))
                    {
                        var indexK = info.IndexOf('=');
                        var name = info.Substring(0, indexK).Trim();
                        var val = info.Substring(indexK + 1);
                        if (index == 0)
                        {
                            cookie.Name = name;
                            cookie.Value = val;
                            continue;
                        }
                        if (name.ToLower().Equals("domain", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Domain = val;
                        }
                        else if (name.ToLower().Equals("expires", StringComparison.OrdinalIgnoreCase))
                        {
                            DateTime.TryParse(val, out var expires);
                            cookie.Expires = expires;
                        }
                        else if (name.ToLower().Equals("path", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Path = val;
                        }
                        else if (name.ToLower().Equals("version", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Version = Convert.ToInt32(val);
                        }
                    }
                    else
                    {
                        if (info.Trim().ToLower().Equals("httponly", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.HttpOnly = true;
                        }
                        else if (info.Trim().ToLower().Equals("secure", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Secure = true;
                        }
                    }
                }
                cookieCollection.Add(cookie);
            }
            return cookieCollection;
        }

        /// <summary>
        /// 获取 Cookies
        /// </summary>
        /// <param name="setCookie"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetCookies(string setCookie, Uri uri)
        {
            //获取所有Cookie
            var strCookies = string.Empty;
            var cookies = GetCookiesByHeader(setCookie);
            foreach (Cookie cookie in cookies)
            {
                //忽略过期Cookie
                if (cookie.Expires < DateTime.Now && cookie.Expires != DateTime.MinValue)
                {
                    continue;
                }
                if (uri.Host.Contains(cookie.Domain))
                {
                    strCookies += $"{cookie.Name}={cookie.Value}; ";
                }
            }
            return strCookies;
        }

        /// <summary>
        /// 通过Name 获取 Cookie Value
        /// </summary>
        /// <param name="setCookie">Cookies</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string GetCookieValueByName(string setCookie, string name)
        {
            var regex = new Regex($"(?<={name}=).*?(?=; )");
            return regex.IsMatch(setCookie) ? regex.Match(setCookie).Value : string.Empty;
        }

        /// <summary>
        /// 通过Name 设置 Cookie Value
        /// </summary>
        /// <param name="setCookie">Cookies</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static string SetCookieValueByName(string setCookie, string name, string value)
        {
            var regex = new Regex($"(?<={name}=).*?(?=; )");
            if (regex.IsMatch(setCookie))
            {
                setCookie = regex.Replace(setCookie, value);
            }
            return setCookie;
        }

        /// <summary>
        /// 通过Name 更新Cookie
        /// </summary>
        /// <param name="oldCookie">原Cookie</param>
        /// <param name="newCookie">更新内容</param>
        /// <param name="name">名字</param>
        /// <returns></returns>
        public static string UpdateCookieValueByName(string oldCookie, string newCookie, string name)
        {
            var regex = new Regex($"(?<={name}=).*?[(?=; )|$]");
            if (regex.IsMatch(oldCookie) && regex.IsMatch(newCookie))
            {
                oldCookie = regex.Replace(oldCookie, regex.Match(newCookie).Value);
            }
            return oldCookie;
        }

        /// <summary>
        /// 根据新Cookie 更新旧的
        /// </summary>
        /// <param name="oldCookie"></param>
        /// <param name="newCookie"></param>
        /// <returns></returns>
        public static string UpdateCookieValue(string oldCookie, string newCookie)
        {
            var list = GetCookiesByHeader(newCookie);
            foreach (Cookie cookie in list)
            {
                var regex = new Regex($"(?<={cookie.Name}=).*?[(?=; )|$]");
                oldCookie = regex.IsMatch(oldCookie) ? regex.Replace(oldCookie, cookie.Value) : $"{cookie.Name}={cookie.Value}; {oldCookie}";
            }
            return oldCookie;
        }



    }
}
