using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.WpfApp.Models
{
    public class DriveTypeItem
    {
        public DriveTypeItem(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public int Id { get; set; }
    }
}
