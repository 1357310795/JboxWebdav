using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NutzCode.Libraries.Web
{
    public interface IStreamFactory
    {
        Task<WebStream> CreateStreamAsync(WebParameters pars, CancellationToken token = new CancellationToken());
        Task<string> GetUrlAsync(string url, string postData, string encoding, string uagent = "", Dictionary<string, string> headers = null);
        WebParameters CreateWebParameters(Uri uri);
    }
}
