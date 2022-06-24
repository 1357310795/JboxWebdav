using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Props;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NWebDav.Server.Stores
{
    public class JboxStoreItem : IStoreItem
    {
        public string Name => throw new NotImplementedException();

        public string UniqueKey => throw new NotImplementedException();

        public IPropertyManager PropertyManager => throw new NotImplementedException();

        public ILockingManager LockingManager => throw new NotImplementedException();

        public Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, Stream source)
        {
            throw new NotImplementedException();
        }
    }
}
