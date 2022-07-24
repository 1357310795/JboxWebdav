using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    public static class Config
    {
        public static long Size_Part = 5 * 1024 * 1024;

        public static bool SharedEnabled { get; set; } = true;

        public static string AppDataDir { get; set; } = Environment.GetEnvironmentVariable("LocalAppData") + "\\JboxWebdav\\";
    }
}
