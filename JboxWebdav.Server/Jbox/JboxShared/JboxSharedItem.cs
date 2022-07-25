using JboxWebdav.Server.Jbox;
using NWebDav.Server;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Logging;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox.JboxShared
{
    public class JboxSharedItem : IStoreItem
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxSharedItem));
        private readonly JboxSharedItemInfo _fileInfo;

        public JboxSharedItem(ILockingManager lockingManager, JboxSharedItemInfo fileInfo)
        {
            LockingManager = lockingManager;
            _fileInfo = fileInfo;
            IsWritable = _fileInfo.AccessMode.CheckAccess(JboxAccessMode.upload);
        }

        public static PropertyManager<JboxSharedItem> DefaultPropertyManager { get; } = new PropertyManager<JboxSharedItem>(new DavProperty<JboxSharedItem>[]
        {
            // RFC-2518 properties
            //new DavCreationDate<JboxStoreItem>
            //{
            //    Getter = (context, item) => item._fileInfo.CreationTimeUtc,
            //    Setter = (context, item, value) =>
            //    {
            //        item._fileInfo.CreationTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            new DavDisplayName<JboxSharedItem>
            {
                Getter = (context, item) => item._fileInfo.GetName()
            },
            new DavGetContentLength<JboxSharedItem>
            {
                Getter = (context, item) => item._fileInfo.Bytes
            },
            new DavGetContentType<JboxSharedItem>
            {
                Getter = (context, item) => item.DetermineContentType()
            },
            new DavGetEtag<JboxSharedItem>
            {
                Getter = (context, item) => item.CalculateEtag()
            },
            new DavGetLastModified<JboxSharedItem>
            {
                Getter = (context, item) => item._fileInfo.Modified,
                Setter = (context, item, value) =>
                {
                    item._fileInfo.Modified = value;
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxSharedItem>
            {
                Getter = (context, item) => null
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreItem>(),
            new DavSupportedLockDefault<JboxSharedItem>(),

            // Hopmann/Lippert collection properties
            // (although not a collection, the IsHidden property might be valuable)
            //new DavExtCollectionIsHidden<JboxStoreItem>
            //{
            //    Getter = (context, item) => (item._fileInfo.IsDisplay)
            //},

            // Win32 extensions
            //new Win32CreationTime<JboxStoreItem>
            //{
            //    Getter = (context, item) => item._fileInfo.CreationTimeUtc,
            //    Setter = (context, item, value) =>
            //    {
            //        item._fileInfo.CreationTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32LastAccessTime<JboxStoreItem>
            //{
            //    Getter = (context, item) => item._fileInfo.LastAccessTimeUtc,
            //    Setter = (context, item, value) =>
            //    {
            //        item._fileInfo.LastAccessTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32LastModifiedTime<JboxStoreItem>
            //{
            //    Getter = (context, item) => item._fileInfo.LastWriteTimeUtc,
            //    Setter = (context, item, value) =>
            //    {
            //        item._fileInfo.LastWriteTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32FileAttributes<JboxStoreItem>
            //{
            //    Getter = (context, item) => item._fileInfo.Attributes,
            //    Setter = (context, item, value) =>
            //    {
            //        item._fileInfo.Attributes = value;
            //        return DavStatusCode.Ok;
            //    }
            //}
        });

        public bool IsWritable { get; }
        public string Name => _fileInfo.GetName();
        public string UniqueKey => _fileInfo.Path;
        public string FullPath => _fileInfo.Path;
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult(_fileInfo.OpenRead());

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, long start, long end) => Task.FromResult(_fileInfo.OpenRead(start, end));

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public async Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext)
        {
            throw new NotImplementedException("Not Supported");
        }

        public override int GetHashCode()
        {
            return _fileInfo.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is JboxSharedItem storeItem))
                return false;
            return storeItem._fileInfo.Path.Equals(_fileInfo.Path, StringComparison.CurrentCultureIgnoreCase);
        }

        private string DetermineContentType()
        {
            return MimeTypeHelper.GetMimeType(_fileInfo.MimeType);
        }

        private string CalculateEtag()
        {
            return "\"" + _fileInfo.Hash + "\"";
        }
    }
}
