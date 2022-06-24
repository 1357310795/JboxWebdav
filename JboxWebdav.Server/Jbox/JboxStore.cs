using NWebDav.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NWebDav.Server.Stores
{
    public class JboxStore : IStore
    {
        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<IStoreItem> GetItemAsync(Uri uri, IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
