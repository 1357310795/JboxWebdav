using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    public static class UrlExtension
    {
        public static string UrlEncode(this string url)
        {
            return Uri.EscapeDataString(url);
        }
        public static string Connect(this IEnumerable<string> iterator,string separator)
        {
            StringBuilder s = new StringBuilder();
            foreach (var i in iterator)
            {
                s.Append(i);
                if (i != iterator.Last())
                    s.Append(separator);
            }
            return s.ToString();
        }

        public static string UrlEncodeByParts(this string url)
        {
            return url.Split('/')
                       .Select(s => s.UrlEncode())
                       .Connect("/");
        }
    }
}
