using Jbox;
using Jbox.Service;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JboxWebdav.Server.Jbox.JboxShared
{
    public class JboxSharedRootCollection : IStoreCollection
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxSharedRootCollection));
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");
        public readonly JboxSharedModel _model;

        public JboxSharedRootCollection(ILockingManager lockingManager, JboxSharedModel model)
        {
            LockingManager = lockingManager;
            _model = model;
        }

        public static PropertyManager<JboxSharedRootCollection> DefaultPropertyManager { get; } = new PropertyManager<JboxSharedRootCollection>(new DavProperty<JboxSharedRootCollection>[]
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
            new DavDisplayName<JboxSharedRootCollection>
            {
                Getter = (context, collection) => collection._model.Name
            },
            new DavGetLastModified<JboxSharedRootCollection>
            {
                Getter = (context, collection) => collection._model.Modified,
                Setter = (context, collection, value) =>
                {
                    collection._model.Modified = value;
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxSharedRootCollection>
            {
                Getter = (context, collection) => new []{s_xDavCollection}
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreCollection>(),
            new DavSupportedLockDefault<JboxSharedRootCollection>(),

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
        public string Name => _model.Name;
        public string UniqueKey => _model.GetHashCode().ToString();
        public string FullPath => "/他人的分享链接/" + _model.Name;

        // Jbox collections (a.k.a. directories don't have their own data)
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
            //// Determine the full path
            //var path = "/" + name;

            //var res = JboxService.GetJboxSharedItemInfo(_model.DeliveryCode, path);

            //if (!res.success)
            //{
            //    // The item doesn't exist
            //    return Task.FromResult<IStoreItem>(null);
            //}

            //if (res.IsDir)
            //{
            //    // Check if it's a directory
            //    return Task.FromResult<IStoreItem>(new JboxSharedCollection(LockingManager, res));
            //}
            //else
            //{
            //    // Check if it's a file
            //    return Task.FromResult<IStoreItem>(new JboxSharedCollection(LockingManager, res));
            //}
        }

        public Task<IEnumerable<IStoreItem>> GetItemsAsync(IHttpContext httpContext)
        {
            IEnumerable<IStoreItem> GetItemsInternal()
            {
                switch (_model.State)
                {
                    case JboxSharedState.invalid:
                        yield return new JboxStaticTxtItem(LockingManager, "/他人的分享链接/", "链接无效.txt", "正文 - 链接无效.txt");
                        break;
                    case JboxSharedState.needpassword:
                        yield return new JboxSharedPasswordInputItem(LockingManager, "/他人的分享链接/", "需要密码.txt", "请输入密码：");
                        break;
                    case JboxSharedState.expired:
                        yield return new JboxStaticTxtItem(LockingManager, "/他人的分享链接/", "来晚了，该分享已过期！.txt", "正文 - 来晚了，该分享已过期！.txt");
                        break;
                    case JboxSharedState.ok:
                        var item3 = JboxService.GetJboxSharedItemInfo(_model.DeliveryCode, "/", _model.Token);
                        if (item3.success)
                        {
                            if (item3.IsDir)
                                yield return new JboxSharedCollection(LockingManager,_model, item3);
                            else
                                yield return new JboxSharedItem(LockingManager, item3);

                            if ((_model.AltName ?? "") == "")
                            {
                                _model.AltName = _model.Name;
                                _model.Name = Common.SanitizeFileName($"{item3.DeliveryCreator}的分享 - {item3.Path.Replace("/", "")} - {item3.DeliveryCode.Substring(0, 6)}");
                            }
                            _model.CreatorUid = item3.DeliveryCreatorUid;
                            JboxShared.Save();
                        }
                        else if (item3.Code.Contains("invalid password/token"))
                        {
                            _model.State = JboxSharedState.needpassword;
                            yield return new JboxSharedPasswordInputItem(LockingManager, "/他人的分享链接/", "需要密码.txt", "请输入密码：");
                        }
                        else if (item3.Code.Contains("expired"))
                        {
                            _model.State = JboxSharedState.expired;
                            yield return new JboxStaticTxtItem(LockingManager, "/他人的分享链接/", "来晚了，该分享已过期！.txt", "正文 - 来晚了，该分享已过期！.txt");
                        }
                        else
                        {
                            _model.State = JboxSharedState.error;
                            yield return new JboxStaticTxtItem(LockingManager, "/他人的分享链接/", "链接无效.txt", "正文 - 链接无效.txt");
                        }
                        break;
                    case JboxSharedState.error:
                        yield return new JboxStaticTxtItem(LockingManager, "/他人的分享链接/", "链接无效.txt", "正文 - 链接无效.txt");
                        break;
                }
            }
            if (_model.State == JboxSharedState.invalid)
                JboxShared.CheckIsValid(_model);
            return Task.FromResult(GetItemsInternal());
        }

        public Task<IStoreItem> GetItemFromPathAsync(string path)
        {
            var folders = path.TrimEnd('/').Split('/').ToList();
            folders.RemoveRange(1, 2);

            var top = folders[1];
            if (_model.State == JboxSharedState.ok && top == "需要密码.txt")
                return Task.FromResult<IStoreItem>(null);
            var res = GetItemsAsync(null).Result;
            if (res == null)
                return Task.FromResult<IStoreItem>(null);
            var res1 = res.FirstOrDefault(x => x.Name == top);
            if (res1 == null)
                return Task.FromResult<IStoreItem>(null);
            if (folders.Count == 2)
                return Task.FromResult(res1);

            if (_model.State == JboxSharedState.ok)
            {
                folders.RemoveAt(1);
                var newpath = string.Join('/', folders);
                var res2 = JboxService.GetJboxSharedItemInfo(_model.DeliveryCode, newpath, _model.Token);
                if (res2.IsDir)
                    return Task.FromResult<IStoreItem>(new JboxSharedCollection(LockingManager, _model, res2));
                else
                    return Task.FromResult<IStoreItem>(new JboxSharedItem(LockingManager, res2));
            }
            else
            {
                return Task.FromResult<IStoreItem>(null);
            }
        }

        public Task<IStoreCollection> GetCollectionFromPathAsync(string path)
        {
            var folders = path.TrimEnd('/').Split('/').ToList();
            folders.RemoveRange(1, 2);

            var top = folders[1];
            var res = GetItemsAsync(null).Result;
            if (res == null)
                return Task.FromResult<IStoreCollection>(null);
            var res1 = res.FirstOrDefault(x => x.Name == top);
            if (res1 == null)
                return Task.FromResult<IStoreCollection>(null);
            if (folders.Count == 2)
                return Task.FromResult<IStoreCollection>(res1 as IStoreCollection);

            if (_model.State == JboxSharedState.ok)
            {
                folders.RemoveAt(1);
                var newpath = string.Join('/', folders);
                var res2 = JboxService.GetJboxSharedItemInfo(_model.DeliveryCode, newpath, _model.Token);
                if (res2.IsDir)
                    return Task.FromResult<IStoreCollection>(new JboxSharedCollection(LockingManager, _model, res2));
                else
                    return Task.FromResult<IStoreCollection>(null);
            }
            else
            {
                return Task.FromResult<IStoreCollection>(null);
            }
        }

        public Task<StoreItemResult> CreateItemAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.Forbidden));
        }

        public async Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, string name, Stream inputStream, long length)
        {
            if (_model.State == JboxSharedState.needpassword && name == "需要密码.txt")
            {
                var sr = new StreamReader(inputStream);
                var content = sr.ReadToEnd();
                var regex = new Regex("请输入密码：([0-9a-zA-Z]{4})");
                var match = regex.Match(content);
                if (match.Success)
                {
                    var password = match.Groups[1].Value;
                    var password_enc = Common.RSAEncrypt(Jac.publicKey, password);
                    var tokenres = JboxService.GetDeliveryAuthToken(_model.DeliveryCode, password_enc);
                    if (tokenres.success)
                    {
                        _model.Token = tokenres.result;
                        _model.State = JboxSharedState.ok;
                        JboxShared.Save();
                        return DavStatusCode.Ok;
                    }
                    else
                        return DavStatusCode.Ok;
                }
                else
                    return DavStatusCode.Ok;
            }
            else
                return DavStatusCode.Forbidden;
        }

        public Task<StoreCollectionResult> CreateCollectionAsync(string name, bool overwrite, IHttpContext httpContext)
        {
            return Task.FromResult(new StoreCollectionResult(DavStatusCode.Forbidden));
        }

        public async Task<StoreItemResult> CopyAsync(IStoreCollection destinationCollection, string name, bool overwrite, IHttpContext httpContext)
        {
            var result = await destinationCollection.CreateCollectionAsync(name, overwrite, httpContext).ConfigureAwait(false);
            return new StoreItemResult(result.Result, result.Collection);
        }

        public bool SupportsFastMove(IStoreCollection destination, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            return true;
        }

        public async Task<StoreItemResult> MoveItemAsync(string sourceName, IStoreCollection destinationCollection, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            // Determine the object that is being moved
            var sourceitem = await GetItemAsync(sourceName, httpContext).ConfigureAwait(false);
            if (sourceitem == null)
                return new StoreItemResult(DavStatusCode.NotFound);
            JboxSharedItemInfo sourceiteminfo = null;
            if (sourceitem is JboxSharedCollection collection)
            {
                sourceiteminfo = collection._shareddirectoryInfo;
            }
            else if (sourceitem is JboxSharedItem item)
            {
                sourceiteminfo = item._fileInfo;
            }
            else
                return new StoreItemResult(DavStatusCode.BadRequest);

            if (!(destinationCollection is JboxStoreCollection))
                return new StoreItemResult(DavStatusCode.BadRequest);

            var res = JboxService.JboxDeliveryTransfer(sourceiteminfo, (JboxStoreCollection)destinationCollection, _model.Token, _model.CreatorUid);

            if (res.success)
                return new StoreItemResult(DavStatusCode.Created);
            else
                return new StoreItemResult(DavStatusCode.BadRequest);
            //try
            //{
            //    if (destinationCollection is not JboxStoreCollection destinationJboxStoreCollection)
            //    {
            //        //// Attempt to copy the item to the destination collection
            //        //var result = await item.CopyAsync(destinationCollection, destinationName, overwrite, httpContext).ConfigureAwait(false);
            //        //if (result.Result == DavStatusCode.Created || result.Result == DavStatusCode.NoContent)
            //        //    await DeleteItemAsync(sourceName, httpContext).ConfigureAwait(false);

            //        //// Return the result
            //        //return result;
            //        throw new Exception("the destination collection is not a directory");
            //    }
            //    else// If the destination collection is a directory too, then we can simply move the file
            //    {
            //        // Return error
            //        if (!destinationJboxStoreCollection.IsWritable)
            //            return new StoreItemResult(DavStatusCode.PreconditionFailed);

            //        // Determine source and destination paths
            //        var sourcePath = UriHelper.Combine(_directoryInfo.Path, sourceName);
            //        var destinationPath = UriHelper.Combine(destinationJboxStoreCollection._directoryInfo.Path, destinationName);

            //        // Check if the file already exists
            //        DavStatusCode result;

            //        var res = JboxService.GetJboxItemInfo(destinationPath);

            //        if (res.success)
            //        {
            //            if (!overwrite)
            //                return new StoreItemResult(DavStatusCode.PreconditionFailed);

            //            JboxService.DeleteJboxItem(res);
            //            result = DavStatusCode.NoContent;
            //        }
            //        else
            //            result = DavStatusCode.Created;

            //        switch (item)
            //        {
            //            case JboxStoreItem _:
            //                // Move the file
            //                if (destinationJboxStoreCollection._directoryInfo.Path == _directoryInfo.Path)
            //                {
            //                    JboxService.RenameJboxItem(sourcePath, destinationPath);
            //                }
            //                else
            //                {
            //                    JboxService.MoveJboxItem(sourcePath, destinationJboxStoreCollection._directoryInfo.Path);
            //                }

            //                var item1 = JboxService.GetJboxItemInfo(destinationPath);
            //                if (!item1.success)
            //                    return new StoreItemResult(DavStatusCode.Conflict);

            //                return new StoreItemResult(result, new JboxStoreItem(LockingManager, item1.ToJboxFileInfo(), IsWritable));

            //            case JboxStoreCollection _:
            //                // Move the directory
            //                if (destinationJboxStoreCollection._directoryInfo.Path == _directoryInfo.Path)
            //                {
            //                    JboxService.RenameJboxItem(sourcePath, destinationPath);
            //                }
            //                else
            //                {
            //                    JboxService.MoveJboxItem(sourcePath, destinationJboxStoreCollection._directoryInfo.Path);
            //                }

            //                var item2 = JboxService.GetJboxItemInfo(destinationPath);
            //                if (!item2.success)
            //                    return new StoreItemResult(DavStatusCode.Conflict);

            //                return new StoreItemResult(result, new JboxStoreCollection(LockingManager, item2.ToJboxDirectoryInfo(), IsWritable));
            //            default:
            //                return new StoreItemResult(DavStatusCode.InternalServerError);
            //        }
            //    }
            //}
            //catch (UnauthorizedAccessException)
            //{
            //    return new StoreItemResult(DavStatusCode.Forbidden);
            //}
        }

        public Task<DavStatusCode> DeleteItemAsync(string name, IHttpContext httpContext)
        {
            return Task.FromResult(DavStatusCode.Forbidden);
        }

        public InfiniteDepthMode InfiniteDepthMode => InfiniteDepthMode.Assume1;

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var storeCollection = obj as JboxStoreCollection;
            if (storeCollection == null)
                return false;
            return storeCollection.FullPath.Equals(FullPath, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
