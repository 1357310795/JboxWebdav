using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    public enum JboxAccessMode
    {
        preview = 1,
        upload = 2,
        download = 4,
        delivery = 24,
        move = 256,
        copy = 512,
        rename = 128,
        delete = 64,
        create = 32
    }
    //39:create:download:upload:preview

    public static class JboxAccess
    {
        public static bool CheckAccess(this int access, JboxAccessMode mode)
        {
            return (access | (int)mode) > 0;
        }
    }
}
