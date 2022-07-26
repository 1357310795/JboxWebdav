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

namespace JboxWebdav.Server.Jbox.JboxPublic
{
    public class JboxPublicItem : IStoreItem
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxStoreItem));
        private readonly JboxItemInfo _itemInfo;

        public JboxPublicItem(ILockingManager lockingManager, JboxItemInfo itemInfo)
        {
            LockingManager = lockingManager;
            _itemInfo = itemInfo;
            IsWritable = false;
        }

        public static PropertyManager<JboxPublicItem> DefaultPropertyManager { get; } = new PropertyManager<JboxPublicItem>(new DavProperty<JboxPublicItem>[]
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
            new DavDisplayName<JboxPublicItem>
            {
                Getter = (context, item) => item._itemInfo.GetName()
            },
            new DavGetContentLength<JboxPublicItem>
            {
                Getter = (context, item) => item._itemInfo.Bytes
            },
            new DavGetContentType<JboxPublicItem>
            {
                Getter = (context, item) => item.DetermineContentType()
            },
            new DavGetEtag<JboxPublicItem>
            {
                Getter = (context, item) => item.CalculateEtag()
            },
            new DavGetLastModified<JboxPublicItem>
            {
                Getter = (context, item) => item._itemInfo.Modified,
                Setter = (context, item, value) =>
                {
                    item._itemInfo.Modified = value;
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxPublicItem>
            {
                Getter = (context, item) => null
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreItem>(),
            new DavSupportedLockDefault<JboxPublicItem>(),

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
        public string Name => _itemInfo.GetName();
        public string UniqueKey => _itemInfo.Path;
        public string FullPath => _itemInfo.Path;
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)JboxService.GetPublicFile(_itemInfo.Path, _itemInfo.Bytes));

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, long start, long end) => Task.FromResult((Stream)JboxService.GetPublicFile(_itemInfo.Path, _itemInfo.Bytes, start, end));

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public async Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext)
        {
            throw new NotImplementedException("Not Supported");
        }

        public override int GetHashCode()
        {
            return _itemInfo.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is JboxPublicItem storeItem))
                return false;
            return storeItem._itemInfo.Path.Equals(_itemInfo.Path, StringComparison.CurrentCultureIgnoreCase);
        }

        private string DetermineContentType()
        {
            return MimeTypeHelper.GetMimeType(_itemInfo.MimeType);
        }

        private string CalculateEtag()
        {
            return "\"" + _itemInfo.Hash + "\"";
        }
    }
}
