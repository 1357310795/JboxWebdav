using Jbox.Service;
using JboxWebdav.Server.Jbox;
using JboxWebdav.Server.Jbox.JboxPublic;
using JboxWebdav.Server.Jbox.JboxShared;
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
    public class JboxStoreCollection : IStoreCollection
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxStoreCollection));
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");
        public readonly JboxDirectoryInfo _directoryInfo;

        public JboxStoreCollection(ILockingManager lockingManager, JboxDirectoryInfo directoryInfo, bool isWritable)
        {
            LockingManager = lockingManager;
            _directoryInfo = directoryInfo;
            IsWritable = isWritable;
        }

        public static PropertyManager<JboxStoreCollection> DefaultPropertyManager { get; } = new PropertyManager<JboxStoreCollection>(new DavProperty<JboxStoreCollection>[]
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
            new DavDisplayName<JboxStoreCollection>
            {
                Getter = (context, collection) => collection._directoryInfo.GetName()
            },
            new DavGetLastModified<JboxStoreCollection>
            {
                Getter = (context, collection) => collection._directoryInfo.Modified,
                Setter = (context, collection, value) =>
                {
                    collection._directoryInfo.Modified = value;
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxStoreCollection>
            {
                Getter = (context, collection) => new []{s_xDavCollection}
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreCollection>(),
            new DavSupportedLockDefault<JboxStoreCollection>(),

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
            new DavQuotaAvailableBytes<JboxStoreCollection>
            {
                Getter = (context, collection) => Jac.userInfo.Quota - Jac.userInfo.Used,
                Setter = (context, collection, value) =>
                {
                    return DavStatusCode.Conflict;
                }
            },
            new DavQuotaUsedBytes<JboxStoreCollection>
            {
                Getter = (context, collection) => Jac.userInfo.Used,
                Setter = (context, collection, value) =>
                {
                    return DavStatusCode.Conflict;
                }
            }
        });

        public bool IsWritable { get; }
        public string Name => _directoryInfo.GetName();
        public string UniqueKey => _directoryInfo.Path;
        public string FullPath => _directoryInfo.Path;

        // Jbox collections (a.k.a. directories don't have their own data)
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)null);

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext,long start, long end) => Task.FromResult((Stream)null);

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public Task<IStoreItem> GetItemAsync(string name, IHttpContext httpContext)
        {
            // Determine the full path
            var fullPath = UriHelper.Combine(_directoryInfo.Path, name);

            var res = JboxService.GetJboxItemInfo(fullPath);

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

        public Task<IEnumerable<IStoreItem>> GetItemsAsync(IHttpContext httpContext)
        {
            IEnumerable<IStoreItem> GetItemsInternal()
            {
                // Add all directories
                foreach (var subDirectory in _directoryInfo.GetDirectories())
                    yield return new JboxStoreCollection(LockingManager, subDirectory, IsWritable);

                // Add all files
                foreach (var file in _directoryInfo.GetFiles())
                    yield return new JboxStoreItem(LockingManager, file, IsWritable);

                if (Config.SharedEnabled && FullPath == "/")
                    yield return JboxSpecialCollection_Shared.getInstance(LockingManager, JboxSpecialCollectionType.Shared);

                if (Config.PublicEnabled && FullPath == "/")
                    yield return JboxSpecialCollection_Public.getInstance(LockingManager);
            }

            return Task.FromResult(GetItemsInternal());
        }

        public Task<StoreItemResult> CreateItemAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            //// Return error
            //if (!IsWritable)
            //    return Task.FromResult(new StoreItemResult(DavStatusCode.PreconditionFailed));

            //// Determine the destination path
            //var destinationPath = Path.Combine(FullPath, name);

            //// Determine result
            //DavStatusCode result;

            //// Check if the file can be overwritten
            //if (File.Exists(name))
            //{
            //    if (!overwrite)
            //        return Task.FromResult(new StoreItemResult(DavStatusCode.PreconditionFailed));

            //    result = DavStatusCode.NoContent;
            //}
            //else
            //{
            //    result = DavStatusCode.Created;
            //}

            //try
            //{
            //    // Create a new file
            //    File.Create(destinationPath).Dispose();
            //}
            //catch (Exception exc)
            //{
            //    // Log exception
            //    s_log.Log(LogLevel.Error, () => $"Unable to create '{destinationPath}' file.", exc);
            //    return Task.FromResult(new StoreItemResult(DavStatusCode.InternalServerError));
            //}

            //// Return result
            //return Task.FromResult(new StoreItemResult(result, new JboxStoreItem(LockingManager, new JboxFileInfo(destinationPath), IsWritable)));
            throw new NotImplementedException("Not Supported");
        }

        public async Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, string name, Stream inputStream, long length)
        {
            // Check if the item is writable
            if (!IsWritable)
                return DavStatusCode.Forbidden;

            // Copy the stream
            try
            {
                // Copy the information to the destination stream
                var res = JboxService.UploadFile(UriHelper.Combine(_directoryInfo.Path, name), inputStream, length);
                if (!res.success)
                    return DavStatusCode.Conflict;
                return DavStatusCode.Ok;
            }
            //catch (IOException ioException) when (ioException.IsJboxFull())
            //{
            //    return DavStatusCode.InsufficientStorage;
            //}
            catch (Exception ex)
            {
                return DavStatusCode.InternalServerError;
            }
        }

        public Task<StoreCollectionResult> CreateCollectionAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            // Return error
            if (!IsWritable)
                return Task.FromResult(new StoreCollectionResult(DavStatusCode.PreconditionFailed));

            // Determine the destination path
            var destinationPath = UriHelper.Combine(FullPath, name);

            // Check if the directory can be overwritten
            DavStatusCode result;
            JboxCreateDirInfo created;
            try
            {
                created = JboxService.CreateDirectory(destinationPath);
                if (created.success)
                    result = DavStatusCode.Created;
                else if (created.Code== "target name exists")
                    result = DavStatusCode.Created;
                else
                    result = DavStatusCode.NoContent;
            }
            catch (Exception exc)
            {
                // Log exception
                s_log.Log(LogLevel.Error, () => $"Unable to create '{destinationPath}' directory.", exc);
                return null;
            }

            // Return the collection
            return Task.FromResult(new StoreCollectionResult(result, new JboxStoreCollection(LockingManager, created.ToJboxDirectoryInfo(), IsWritable)));
            //throw new NotImplementedException("Not Supported");
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
            // Return error
            if (!IsWritable)
                return new StoreItemResult(DavStatusCode.PreconditionFailed);

            // Determine the object that is being moved
            var item = await GetItemAsync(sourceName, httpContext).ConfigureAwait(false);
            if (item == null)
                return new StoreItemResult(DavStatusCode.NotFound);

            try
            {
                if (destinationCollection is not JboxStoreCollection destinationJboxStoreCollection)
                {
                    //// Attempt to copy the item to the destination collection
                    //var result = await item.CopyAsync(destinationCollection, destinationName, overwrite, httpContext).ConfigureAwait(false);
                    //if (result.Result == DavStatusCode.Created || result.Result == DavStatusCode.NoContent)
                    //    await DeleteItemAsync(sourceName, httpContext).ConfigureAwait(false);

                    //// Return the result
                    //return result;
                    throw new Exception("the destination collection is not a directory");
                }
                else// If the destination collection is a directory too, then we can simply move the file
                {
                    // Return error
                    if (!destinationJboxStoreCollection.IsWritable)
                        return new StoreItemResult(DavStatusCode.PreconditionFailed);

                    // Determine source and destination paths
                    var sourcePath = UriHelper.Combine(_directoryInfo.Path, sourceName);
                    var destinationPath = UriHelper.Combine(destinationJboxStoreCollection._directoryInfo.Path, destinationName);

                    // Check if the file already exists
                    DavStatusCode result;

                    var res = JboxService.GetJboxItemInfo(destinationPath);

                    if (res.success)
                    {
                        if (!overwrite)
                            return new StoreItemResult(DavStatusCode.PreconditionFailed);

                        JboxService.DeleteJboxItem(res);
                        result = DavStatusCode.NoContent;
                    }
                    else
                        result = DavStatusCode.Created;

                    switch (item)
                    {
                        case JboxStoreItem _:
                            // Move the file
                            if (destinationJboxStoreCollection._directoryInfo.Path == _directoryInfo.Path)
                            {
                                JboxService.RenameJboxItem(sourcePath, destinationPath);
                            }
                            else
                            {
                                JboxService.MoveJboxItem(sourcePath, destinationJboxStoreCollection._directoryInfo.Path);
                            }
                            
                            var item1 = JboxService.GetJboxItemInfo(destinationPath);
                            if (!item1.success)
                                return new StoreItemResult(DavStatusCode.Conflict);

                            return new StoreItemResult(result, new JboxStoreItem(LockingManager, item1.ToJboxFileInfo(), IsWritable));

                        case JboxStoreCollection _:
                            // Move the directory
                            if (destinationJboxStoreCollection._directoryInfo.Path == _directoryInfo.Path)
                            {
                                JboxService.RenameJboxItem(sourcePath, destinationPath);
                            }
                            else
                            {
                                JboxService.MoveJboxItem(sourcePath, destinationJboxStoreCollection._directoryInfo.Path);
                            }

                            var item2 = JboxService.GetJboxItemInfo(destinationPath);
                            if (!item2.success)
                                return new StoreItemResult(DavStatusCode.Conflict);

                            return new StoreItemResult(result, new JboxStoreCollection(LockingManager, item2.ToJboxDirectoryInfo(), IsWritable));
                        default:
                            return new StoreItemResult(DavStatusCode.InternalServerError);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                return new StoreItemResult(DavStatusCode.Forbidden);
            }
        }

        public Task<DavStatusCode> DeleteItemAsync(string name, IHttpContext httpContext)
        {
            // Return error
            if (!IsWritable)
                return Task.FromResult(DavStatusCode.PreconditionFailed);

            // Determine the full path
            var fullPath = UriHelper.Combine(_directoryInfo.Path, name);
            try
            {
                // Check if the item exists
                var item = JboxService.GetJboxItemInfo(fullPath);
                if (item.success)
                {
                    JboxService.DeleteJboxItem(fullPath);
                    return Task.FromResult(DavStatusCode.Ok);
                }
                // Item not found
                return Task.FromResult(DavStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return Task.FromResult(DavStatusCode.Forbidden);
            }
            catch (Exception exc)
            {
                // Log exception
                s_log.Log(LogLevel.Error, () => $"Unable to delete '{fullPath}' directory.", exc);
                return Task.FromResult(DavStatusCode.BadRequest);
            }
        }

        public InfiniteDepthMode InfiniteDepthMode => InfiniteDepthMode.Assume1;

        public override int GetHashCode()
        {
            return _directoryInfo.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var storeCollection = obj as JboxStoreCollection;
            if (storeCollection == null)
                return false;
            return storeCollection._directoryInfo.Path.Equals(_directoryInfo.Path, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
