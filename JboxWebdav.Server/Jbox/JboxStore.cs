using Jbox.Service;
using JboxWebdav.Server.Jbox;
using JboxWebdav.Server.Jbox.JboxPublic;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Logging;

namespace NWebDav.Server.Stores
{
    public class JboxStore : IStore
    {
        private static ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxStore));
        public JboxStore()
        {
            IsWritable = true;
            LockingManager = new InMemoryLockingManager();
        }

        public JboxStore(JboxCookie cookie, bool isWritable = true, ILockingManager lockingManager = null)
        {
            Cookie = cookie;
            IsWritable = isWritable;
            LockingManager = lockingManager ?? new InMemoryLockingManager();
        }

        public JboxCookie Cookie { get; }
        public bool IsWritable { get; }
        public ILockingManager LockingManager { get; }

        public Task<IStoreItem> GetItemAsync(Uri uri, IHttpContext httpContext)
        {
            var res = GetItemAsyncInternal(uri).Result;
            s_log.Log(LogLevel.Debug, () => $"【{(res?.GetType())}】路径 {uri.ToString()}");
            return Task.FromResult(res);
        }

        private Task<IStoreItem> GetItemAsyncInternal(Uri uri)
        {
            // Determine the path from the uri
            var path = UriHelper.GetPathFromUri(uri);
            var topfolder = UriHelper.GetTopFolderFromUri(uri);

            if (topfolder == "他人的分享链接")
            {
                var specialfolder = JboxSpecialCollection_Shared.getInstance(LockingManager, JboxSpecialCollectionType.Shared);
                return specialfolder.GetItemFromPathAsync(path);
                //return Task.FromResult<IStoreItem>();
            }
            if (topfolder == "交大空间")
            {
                var specialfolder = JboxSpecialCollection_Public.getInstance(LockingManager);
                return specialfolder.GetItemFromPathAsync(path);
                //return Task.FromResult<IStoreItem>();
            }

            var res = JboxService.GetJboxItemInfo(path);

            if (!res.success)
            {
                // The item doesn't exist
                return Task.FromResult<IStoreItem>(null);
            }

            if (res.IsDir)
            {
                // Check if it's a directory
                return Task.FromResult<IStoreItem>(new JboxStoreCollection(LockingManager, res.ToJboxDirectoryInfo(), IsWritable));
            }
            else
            {
                // Check if it's a file
                return Task.FromResult<IStoreItem>(new JboxStoreItem(LockingManager, res.ToJboxFileInfo(), IsWritable));
            }
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext)
        {
            var res = GetCollectionInternal(uri).Result;
            s_log.Log(LogLevel.Debug, () => $"【{(res?.GetType())}】路径 {uri.ToString()}");
            return Task.FromResult(res);
        }

        private Task<IStoreCollection> GetCollectionInternal(Uri uri)
        {
            // Determine the path from the uri
            var path = UriHelper.GetPathFromUri(uri);
            var topfolder = UriHelper.GetTopFolderFromUri(uri);

            if (topfolder == "他人的分享链接")
            {
                var specialfolder = JboxSpecialCollection_Shared.getInstance(LockingManager, JboxSpecialCollectionType.Shared);
                return specialfolder.GetCollectionFromPathAsync(path);
                //return Task.FromResult<IStoreItem>();
            }

            if (topfolder == "交大空间")
            {
                var specialfolder = JboxSpecialCollection_Public.getInstance(LockingManager);
                return specialfolder.GetCollectionFromPathAsync(path);
                //return Task.FromResult<IStoreItem>();
            }

            var res = JboxService.GetJboxItemInfo(path);

            if (!res.success || !res.IsDir)
            {
                // The item doesn't exist
                return Task.FromResult<IStoreCollection>(null);
            }

            // Return the item
            return Task.FromResult<IStoreCollection>(new JboxStoreCollection(LockingManager, res.ToJboxDirectoryInfo(), IsWritable));
        }
    }
}
