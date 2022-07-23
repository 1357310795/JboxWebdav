using NWebDav.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    public interface IStorage
    {
        string GetKeyValue(string Section, string Key, string DefaultText);

        bool SetKeyValue(string Section, string Key, string Value);

        bool DelKeyValue(string Section, string Key);

        bool DelSection(string Section);
    }
}
