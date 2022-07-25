using Jbox.Helpers;
using Jbox.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Jbox
{
    public static class Common
    {
        public static long GetTimeStampInt64()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static string GetTimeStampMilli()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;

        }

        public static string GetSHA256(Stream stream)
        {
            string result = string.Empty;
            byte[] by = null;

            stream.Position = 0;
            var s = System.Security.Cryptography.SHA256.Create();
            by = s.ComputeHash(stream);

            result = BitConverter.ToString(by).Replace("-", "").ToLower();
            return result;
        }

        public static string RSAEncrypt(string publicKey, string enptStr)
        {
            RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
            var xmlString = rsa.ToXmlString(false);
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlString);
            var result = provider.Encrypt(Encoding.Default.GetBytes(enptStr), false);
            return Convert.ToBase64String(result);
        }

        public static Dictionary<string, string> ParseQueryString(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url");
            }
            var uri = new Uri(url);
            if (string.IsNullOrWhiteSpace(uri.Query))
            {
                return new Dictionary<string, string>();
            }
            //1.去除第一个前导?字符
            var dic = uri.Query.Substring(1)
                    //2.通过&划分各个参数
                    .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                    //3.通过=划分参数key和value,且保证只分割第一个=字符
                    .Select(param => param.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                    //4.通过相同的参数key进行分组
                    .GroupBy(part => part[0], part => part.Length > 1 ? part[1] : string.Empty)
                    //5.将相同key的value以,拼接
                    .ToDictionary(group => group.Key, group => string.Join(",", group));

            return dic;
        }

    }
}
