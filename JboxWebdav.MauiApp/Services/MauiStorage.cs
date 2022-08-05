using JboxWebdav.Server.Jbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.MauiApp.Services
{
    public class MauiStorage : IStorage
    {
        public bool DelKeyValue(string Section, string Key)
        {
            throw new NotImplementedException();
        }

        public bool DelSection(string Section)
        {
            throw new NotImplementedException();
        }

        public string GetKeyValue(string Section, string Key, string DefaultText)
        {
            string res = SecureStorage.Default.GetAsync($"{Section}_{Key}").Result;
            return res ?? DefaultText;
        }

        public bool SetKeyValue(string Section, string Key, string Value)
        {
            var task = Task.Run(async () => { await SecureStorage.Default.SetAsync($"{Section}_{Key}", Value); });
            task.Wait();
            return true;
        }
    }
}
