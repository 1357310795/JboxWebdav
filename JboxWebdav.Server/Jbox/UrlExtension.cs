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
            var it =iterator.GetEnumerator();
            it.MoveNext();
            bool next;
            do
            {
                
                s.Append(it.Current);
                next = it.MoveNext();
                if (next)
                    s.Append(separator);

            } while (next);

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
