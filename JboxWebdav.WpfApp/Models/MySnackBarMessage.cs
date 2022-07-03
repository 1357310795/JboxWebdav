using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.WpfApp.Models
{
    public class MySnackBarMessage
    {
        public string message;
        public TimeSpan duration;

        public MySnackBarMessage(string message, TimeSpan duration)
        {
            this.message = message;
            this.duration = duration;
        }
    }
}
