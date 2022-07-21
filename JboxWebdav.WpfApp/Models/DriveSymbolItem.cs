using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.WpfApp.Models
{
    public class DriveSymbolItem
    {
        public DriveSymbolItem(string desc, string id)
        {
            Desc = desc;
            Id = id;
        }

        public DriveSymbolItem(string id)
        {
            Desc = id + ":";
            Id = id;
        }

        public string Desc { get; set; }
        public string Id { get; set; }
    }
}
