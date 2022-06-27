using Jbox.Service;
using JboxWebdav.Server.Jbox;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;

namespace NWebDav.Server.Stores
{
    public class JboxStore : IStore//ok
    {
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
            // Determine the path from the uri
            var path = UriHelper.GetPathFromUri(uri);

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
            // Determine the path from the uri
            var path = UriHelper.GetPathFromUri(uri);

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
