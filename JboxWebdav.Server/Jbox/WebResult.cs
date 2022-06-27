using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jbox.Models
{
    public class WebResult : CommonResult
    {
        public HttpStatusCode? code;

        public WebResult(HttpStatusCode? code, bool success, string result)
        {
            this.code = code;
            this.success = success;
            this.result = result;
        }
    }
}
