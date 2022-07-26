using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Logging;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;
using System.Xml.Linq;

namespace JboxWebdav.Server.Jbox.JboxPublic
{
    public class JboxSpecialCollection_Public : IStoreCollection
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxSpecialCollection_Public));
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");

        private static JboxSpecialCollection_Public _instance;
        private static readonly object synchronized = new object();

        public static JboxSpecialCollection_Public getInstance(ILockingManager lockingManager)
        {
            if (_instance == null)
            {
                lock (synchronized)  //加锁防止多线程
                {
                    if (_instance == null)
                    {
                        _instance = new JboxSpecialCollection_Public(lockingManager);
                    }
                }
            }
            return _instance;
        }

        public JboxSpecialCollection_Public(ILockingManager lockingManager)
        {
            LockingManager = lockingManager;
            IsWritable = false;
        }

        public static PropertyManager<JboxSpecialCollection_Public> DefaultPropertyManager { get; } = new PropertyManager<JboxSpecialCollection_Public>(new DavProperty<JboxSpecialCollection_Public>[]
        {
            // RFC-2518 properties
            //new DavCreationDate<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.CreationTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.CreationTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            new DavDisplayName<JboxSpecialCollection_Public>
            {
                Getter = (context, collection) => "交大空间"
            },
            new DavGetLastModified<JboxSpecialCollection_Public>
            {
                Getter = (context, collection) => DateTime.Now,
                Setter = (context, collection, value) =>
                {
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxSpecialCollection_Public>
            {
                Getter = (context, collection) => new []{s_xDavCollection}
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreCollection>(),
            new DavSupportedLockDefault<JboxSpecialCollection_Public>(),

            // Hopmann/Lippert collection properties
            //new DavExtCollectionChildCount<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.ItemCount
            //},
            //new DavExtCollectionIsFolder<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => true
            //},
            //new DavExtCollectionIsHidden<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => (collection._directoryInfo.IsDisplay)
            //},
            //new DavExtCollectionIsStructuredDocument<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => false
            //},
            //new DavExtCollectionHasSubs<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => (collection._directoryInfo.DirectoryCount > 0)
            //},
            //new DavExtCollectionNoSubs<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => false
            //},
            //new DavExtCollectionObjectCount<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.FileCount
            //},
            //new DavExtCollectionReserved<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => !collection.IsWritable
            //},
            //new DavExtCollectionVisibleCount<JboxStoreCollection>
            //{
            //    Getter = (context, collection) =>
            //        collection._directoryInfo.VisibleCount
            //},

            // Win32 extensions
            //new Win32CreationTime<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.CreationTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.CreationTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32LastAccessTime<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.LastAccessTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.LastAccessTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32LastModifiedTime<JboxStoreCollection>
            //{
            //    Getter = (context, collection) => collection._directoryInfo.LastWriteTimeUtc,
            //    Setter = (context, collection, value) =>
            //    {
            //        collection._directoryInfo.LastWriteTimeUtc = value;
            //        return DavStatusCode.Ok;
            //    }
            //},
            //new Win32FileAttributes<JboxStoreCollection>
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
        public string Name => "交大空间";
        public string UniqueKey => "交大空间";
        public string FullPath => "/交大空间";

        // Jbox collections (a.k.a. directories don't have their own data)
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)null);

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext,long start, long end) => Task.FromResult((Stream)null);

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public Task<IStoreItem> GetItemAsync(string name, IHttpContext httpContext)
        {
            // Determine the full path
            var fullPath = $"/{name}";

            var res = JboxService.GetJboxPublicItemInfo(fullPath);

            if (!res.success)
            {
                // The item doesn't exist
                return Task.FromResult<IStoreItem>(null);
            }

            if (res.IsDir)
            {
                // Check if it's a directory
                return Task.FromResult<IStoreItem>(new JboxPublicCollection(LockingManager, res));
            }
            else
            {
                // Check if it's a file
                return Task.FromResult<IStoreItem>(new JboxPublicItem(LockingManager, res));
            }
        }

        public Task<IStoreItem> GetItemFromPathAsync(string path)
        {
            var folders = path.TrimEnd('/').Split('/').ToList();
            if (folders.Count == 2)
                return Task.FromResult<IStoreItem>(this);
            folders.RemoveAt(1);
            var newpath = string.Join('/', folders);
            var res = JboxService.GetJboxPublicItemInfo(newpath);

            if (!res.success)
            {
                return Task.FromResult<IStoreItem>(null);
            }

            if (res.IsDir)
            {
                return Task.FromResult<IStoreItem>(new JboxPublicCollection(LockingManager, res));
            }
            else
            {
                return Task.FromResult<IStoreItem>(new JboxPublicItem(LockingManager, res));
            }
        }

        public Task<IStoreCollection> GetCollectionFromPathAsync(string path)
        {
            var res = GetItemFromPathAsync(path).Result;
            return Task.FromResult<IStoreCollection>(res as IStoreCollection);
        }

        public Task<IEnumerable<IStoreItem>> GetItemsAsync(IHttpContext httpContext)
        {
            IEnumerable<IStoreItem> GetItemsInternal()
            {
                var _itemInfo = JboxService.GetJboxPublicItemInfo("/");
                // Add all directories
                foreach (var subDirectory in _itemInfo.GetDirectories())
                    yield return new JboxPublicCollection(LockingManager, subDirectory);

                // Add all files
                foreach (var file in _itemInfo.GetFiles())
                    yield return new JboxPublicItem(LockingManager, file);

            }

            return Task.FromResult(GetItemsInternal());
        }

        public Task<StoreItemResult> CreateItemAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            throw new NotImplementedException("Not Supported");
        }

        public async Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, string name, Stream inputStream, long length)
        {
            return DavStatusCode.Forbidden;
        }

        public Task<StoreCollectionResult> CreateCollectionAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            return Task.FromResult(new StoreCollectionResult(DavStatusCode.PreconditionFailed));
        }

        public async Task<StoreItemResult> CopyAsync(IStoreCollection destinationCollection, string name, bool overwrite, IHttpContext httpContext)
        {
            // Just create the folder itself
            var result = await destinationCollection.CreateCollectionAsync(name, overwrite, httpContext).ConfigureAwait(false);
            return new StoreItemResult(result.Result, result.Collection);
            throw new NotImplementedException("Not Supported");
        }

        public bool SupportsFastMove(IStoreCollection destination, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            return true;
        }

        public async Task<StoreItemResult> MoveItemAsync(string sourceName, IStoreCollection destinationCollection, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            return new StoreItemResult(DavStatusCode.PreconditionFailed);
        }

        public Task<DavStatusCode> DeleteItemAsync(string name, IHttpContext httpContext)
        {
            return Task.FromResult(DavStatusCode.PreconditionFailed);
        }

        public InfiniteDepthMode InfiniteDepthMode => InfiniteDepthMode.Assume1;

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var storeCollection = obj as JboxSpecialCollection_Public;
            if (storeCollection == null)
                return false;
            return true;
        }
    }
}
