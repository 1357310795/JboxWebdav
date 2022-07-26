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
using System.Xml.Linq;

namespace JboxWebdav.Server.Jbox.JboxPublic
{
    public class JboxPublicCollection : IStoreCollection
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxPublicCollection));
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");
        public readonly JboxItemInfo _directoryInfo;

        public JboxPublicCollection(ILockingManager lockingManager, JboxItemInfo directoryInfo)
        {
            LockingManager = lockingManager;
            _directoryInfo = directoryInfo;
            IsWritable = false;
        }

        public static PropertyManager<JboxPublicCollection> DefaultPropertyManager { get; } = new PropertyManager<JboxPublicCollection>(new DavProperty<JboxPublicCollection>[]
        {
            // RFC-2518 properties
            //new DavCreationDate<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.CreationTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.CreationTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            new DavDisplayName<JboxPublicCollection>
            {
                Getter = (context, collection) => collection._directoryInfo.GetName()
            },
            new DavGetLastModified<JboxPublicCollection>
            {
                Getter = (context, collection) => collection._directoryInfo.Modified,
                Setter = (context, collection, value) =>
                {
                    collection._directoryInfo.Modified = value;
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxPublicCollection>
            {
                Getter = (context, collection) => new []{s_xDavCollection}
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreCollection>(),
            new DavSupportedLockDefault<JboxPublicCollection>(),

            // Hopmann/Lippert collection properties
            //new DavExtCollectionChildCount<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.ItemCount
            //},
            //new DavExtCollectionIsFolder<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => true
            //},
            //new DavExtCollectionIsHidden<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => (collection._directoryInfo.IsDisplay)
            //},
            //new DavExtCollectionIsStructuredDocument<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => false
            //},
            //new DavExtCollectionHasSubs<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => (collection._directoryInfo.DirectoryCount > 0)
            //},
            //new DavExtCollectionNoSubs<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => false
            //},
            //new DavExtCollectionObjectCount<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.FileCount
            //},
            //new DavExtCollectionReserved<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => !collection.IsWritable
            //},
            //new DavExtCollectionVisibleCount<JboxSharedCollection>
            //{
            //    Getter = (context, collection) =>
            //        collection._directoryInfo.VisibleCount
            //},

            // Win32 extensions
            //new Win32CreationTime<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.CreationTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.CreationTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32LastAccessTime<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.LastAccessTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.LastAccessTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32LastModifiedTime<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.LastWriteTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.LastWriteTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32FileAttributes<JboxSharedCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.Attributes,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.Attributes = value;
            //        return DavStatusCode.Ok;
            //    }
            //}
        });

        public bool IsWritable { get; }
        public string Name => _directoryInfo.GetName();
        public string UniqueKey => _directoryInfo.Path;
        public string FullPath => _directoryInfo.Path;

        // directories don't have their own data
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)null);

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, long start, long end) => Task.FromResult((Stream)null);

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public Task<IStoreItem> GetItemAsync(string name, IHttpContext httpContext)
        {
            var res = GetItemsAsync(null).Result;
            if (res == null)
                return Task.FromResult<IStoreItem>(null);
            var res1 = res.FirstOrDefault(x => x.Name == name);
            return Task.FromResult<IStoreItem>(res1);
        }

        public Task<IEnumerable<IStoreItem>> GetItemsAsync(IHttpContext httpContext)
        {
            IEnumerable<IStoreItem> GetItemsInternal()
            {
                // Add all directories
                foreach (var subDirectory in _directoryInfo.GetDirectories())
                    yield return new JboxPublicCollection(LockingManager, subDirectory);

                // Add all files
                foreach (var file in _directoryInfo.GetFiles())
                    yield return new JboxPublicItem(LockingManager, file);
            }

            return Task.FromResult(GetItemsInternal());
        }

        public Task<StoreItemResult> CreateItemAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.Forbidden));
        }

        public async Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, string name, Stream inputStream, long length)
        {
            return DavStatusCode.Forbidden;
        }

        public Task<StoreCollectionResult> CreateCollectionAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            return Task.FromResult(new StoreCollectionResult(DavStatusCode.Forbidden));
        }

        public async Task<StoreItemResult> CopyAsync(IStoreCollection destinationCollection, string name, bool overwrite, IHttpContext httpContext)
        {
            // Just create the folder itself
            var result = await destinationCollection.CreateCollectionAsync(name, overwrite, httpContext).ConfigureAwait(false);
            return new StoreItemResult(result.Result, result.Collection);
        }

        public bool SupportsFastMove(IStoreCollection destination, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            return true;
        }

        public async Task<StoreItemResult> MoveItemAsync(string sourceName, IStoreCollection destinationCollection, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            //Todo
            return new StoreItemResult(DavStatusCode.PreconditionFailed);
        }

        public Task<DavStatusCode> DeleteItemAsync(string name, IHttpContext httpContext)
        {
            return Task.FromResult(DavStatusCode.Forbidden);
        }

        public InfiniteDepthMode InfiniteDepthMode => InfiniteDepthMode.Assume1;

        public override int GetHashCode()
        {
            return _directoryInfo.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var storeCollection = obj as JboxPublicCollection;
            if (storeCollection == null)
                return false;
            return storeCollection._directoryInfo.Path.Equals(_directoryInfo.Path, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
