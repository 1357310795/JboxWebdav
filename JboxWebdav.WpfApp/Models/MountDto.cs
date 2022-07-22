using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.WpfApp.Models
{
    public class MountDto
    {
        public List<MountPoints> mountPoints { get; set; }
    }

    public class MountPoints
    {
        public string Fs { get; set; }
        public string MountPoint { get; set; }
    }
}
