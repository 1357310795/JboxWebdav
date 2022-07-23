using JboxWebdav.Server.Jbox;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.WpfApp.Services
{
    public class WinStorage : IStorage
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int WritePrivateProfileString(string section, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int WritePrivateProfileSection(string section, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        public string GetKeyValue(string Section, string Key, string DefaultText)
        {
            int BufferSize = 9999;
            StringBuilder keyValue = new StringBuilder(BufferSize);
            string text = "";
            int Rvalue = GetPrivateProfileString(Section, Key, text, keyValue, BufferSize, inipath);

            bool flag = Rvalue == 0;
            if (flag)
            {
                return DefaultText;
            }
            else
            {
                return keyValue.ToString();
            }
        }

        public bool SetKeyValue(string Section, string Key, string Value)
        {
            string pat = Path.GetDirectoryName(inipath);
            bool flag = !Directory.Exists(pat);
            if (flag)
            {
                Directory.CreateDirectory(pat);
            }
            bool flag2 = !File.Exists(inipath);
            if (flag2)
            {
                File.Create(inipath).Close();
            }
            int OpStation = WritePrivateProfileString(Section, Key, Value, inipath);
            bool flag3 = OpStation == 0L;
            return !flag3;
        }

        public bool DelKeyValue(string Section, string Key)
        {
            int OpStation = WritePrivateProfileString(Section, Key, null, inipath);
            bool flag3 = OpStation == 0L;
            return !flag3;
        }

        public bool DelSection(string Section)
        {
            int OpStation = WritePrivateProfileSection(Section, null, inipath);
            bool flag3 = OpStation == 0L;
            return !flag3;
        }

        public static string inipath => Environment.GetEnvironmentVariable("LocalAppData") + "\\JboxWebdav\\Settings.ini";
    }
}
