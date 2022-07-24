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
using System.Xml.Linq;

namespace NWebDav.Server.Stores
{
    public class JboxSpecialCollection : IStoreCollection
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxSpecialCollection));
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");
        private readonly JboxSpecialCollectionType _type;

        public JboxSpecialCollection(ILockingManager lockingManager, JboxSpecialCollectionType type)
        {
            LockingManager = lockingManager;
            _type = type;
        }

        public static PropertyManager<JboxSpecialCollection> DefaultPropertyManager { get; } = new PropertyManager<JboxSpecialCollection>(new DavProperty<JboxSpecialCollection>[]
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
            new DavDisplayName<JboxSpecialCollection>
            {
                Getter = (context, collection) => "他人的分享链接"
            },
            new DavGetLastModified<JboxSpecialCollection>
            {
                Getter = (context, collection) => DateTime.Now,
                Setter = (context, collection, value) =>
                {
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxSpecialCollection>
            {
                Getter = (context, collection) => new []{s_xDavCollection}
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreCollection>(),
            new DavSupportedLockDefault<JboxSpecialCollection>(),

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
        public string Name => "他人的分享链接";
        public string UniqueKey => "他人的分享链接";
        public string FullPath => "/他人的分享链接";

        // Jbox collections (a.k.a. directories don't have their own data)
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)null);

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext,long start, long end) => Task.FromResult((Stream)null);

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public Task<IStoreItem> GetItemAsync(string name, IHttpContext httpContext)
        {
            var res = GetItemsAsync(null).Result;
            if (res == null)
                return Task.FromResult<IStoreItem>(null);
            var res1 = res.FirstOrDefault(x=>x.Name == name);
            return Task.FromResult<IStoreItem>(res1);
        }

        public Task<IEnumerable<IStoreItem>> GetItemsAsync(IHttpContext httpContext)
        {
            IEnumerable<IStoreItem> GetItemsInternal()
            {
                // Add all directories
                foreach (var subDirectory in JboxShared.Get())
                    yield return new JboxSharedRootCollection(LockingManager, subDirectory);
            }

            return Task.FromResult(GetItemsInternal());
        }

        public Task<StoreItemResult> CreateItemAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.Conflict));
        }

        public async Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, string name, Stream inputStream, long length)
        {
            return DavStatusCode.Conflict;
        }

        public Task<StoreCollectionResult> CreateCollectionAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            JboxSharedModel model = new JboxSharedModel();
            model.DeliveryCode = "";
            model.State = JboxSharedState.invalid;
            model.Name = name;
            model.Modified = DateTime.Now;

            JboxShared.items.Add(model);
            JboxShared.Save();

            return Task.FromResult(new StoreCollectionResult(DavStatusCode.Created, new JboxSharedRootCollection(LockingManager, model)));
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
            if (destinationCollection == this)
            {
                var list = JboxShared.Get();
                var item = list.FirstOrDefault(x => x.Name == sourceName);
                if (item == null)
                    return new StoreItemResult(DavStatusCode.NotFound);
                item.Name = destinationName;
                item.State = JboxSharedState.invalid;
                return new StoreItemResult(DavStatusCode.Ok, new JboxSharedRootCollection(LockingManager, item));
            }
            else
                return new StoreItemResult(DavStatusCode.Conflict);
        }

        public Task<DavStatusCode> DeleteItemAsync(string name, IHttpContext httpContext)
        {
            var list = JboxShared.Get();
            var item = list.FirstOrDefault(x => x.Name == name);
            if (item == null)
                return Task.FromResult(DavStatusCode.NotFound);
            else
            {
                JboxShared.items.Remove(item);
                JboxShared.Save();
                return Task.FromResult(DavStatusCode.Ok);
            }
        }

        public InfiniteDepthMode InfiniteDepthMode => InfiniteDepthMode.Assume1;

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var storeCollection = obj as JboxSpecialCollection;
            if (storeCollection == null)
                return false;
            return storeCollection.FullPath.Equals(FullPath, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
