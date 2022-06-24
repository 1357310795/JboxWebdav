using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQiangke.Models
{
    public static class WebResult
    {
        public class PostResult : CommonResult
        {
            public PostResult(bool success, string result) : base(success, result)
            {
            }
        }

        public class GetResult : CommonResult
        {
            public GetResult(bool success, string result) : base(success, result)
            {
            }
        }
    }
}
