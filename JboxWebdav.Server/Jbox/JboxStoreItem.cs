using JboxWebdav.Server.Jbox;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Logging;
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
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxStoreItem));
        private readonly JboxFileInfo _fileInfo;

        public JboxStoreItem(ILockingManager lockingManager, JboxFileInfo fileInfo, bool isWritable)
        {
            LockingManager = lockingManager;
            _fileInfo = fileInfo;
            IsWritable = isWritable;
        }

        public static PropertyManager<JboxStoreItem> DefaultPropertyManager { get; } = new PropertyManager<JboxStoreItem>(new DavProperty<JboxStoreItem>[]
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
            new DavDisplayName<JboxStoreItem>
            {
                Getter = (context, item) => item._fileInfo.GetName()
            },
            new DavGetContentLength<JboxStoreItem>
            {
                Getter = (context, item) => item._fileInfo.Bytes
            },
            new DavGetContentType<JboxStoreItem>
            {
                Getter = (context, item) => item.DetermineContentType()
            },
            new DavGetEtag<JboxStoreItem>
            {
                Getter = (context, item) => item.CalculateEtag()
            },
            new DavGetLastModified<JboxStoreItem>
            {
                Getter = (context, item) => item._fileInfo.Modified,
                Setter = (context, item, value) =>
                {
                    item._fileInfo.Modified = value;
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxStoreItem>
            {
                Getter = (context, item) => null
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreItem>(),
            new DavSupportedLockDefault<JboxStoreItem>(),

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
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)_fileInfo.OpenRead());

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, long start, long end) => Task.FromResult((Stream)_fileInfo.OpenRead(start, end));

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
            if (!(obj is JboxStoreItem storeItem))
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
