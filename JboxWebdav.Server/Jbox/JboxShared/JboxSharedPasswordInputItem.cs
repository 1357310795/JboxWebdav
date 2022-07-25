using JboxWebdav.Server.Jbox;
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
using System.Text;
using System.Threading.Tasks;

namespace NWebDav.Server.Stores
{
    public class JboxSharedPasswordInputItem : IStoreItem
    {
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(JboxStaticTxtItem));
        private readonly string _name;
        private readonly byte[] _content;
        private readonly string _path;
        //private readonly JboxSharedModel _model;

        public JboxSharedPasswordInputItem(ILockingManager lockingManager, string path, string name, string content)
        {
            LockingManager = lockingManager;
            _name = name;
            _path = path;
            //_model = model;
            _content = Encoding.Default.GetBytes(content);
        }

        public static PropertyManager<JboxSharedPasswordInputItem> DefaultPropertyManager { get; } = new PropertyManager<JboxSharedPasswordInputItem>(new DavProperty<JboxSharedPasswordInputItem>[]
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
            new DavDisplayName<JboxSharedPasswordInputItem>
            {
                Getter = (context, item) => item._name
            },
            new DavGetContentLength<JboxSharedPasswordInputItem>
            {
                Getter = (context, item) => item._content.Length
            },
            new DavGetContentType<JboxSharedPasswordInputItem>
            {
                Getter = (context, item) => item.DetermineContentType()
            },
            new DavGetEtag<JboxSharedPasswordInputItem>
            {
                Getter = (context, item) => item.CalculateEtag()
            },
            new DavGetLastModified<JboxSharedPasswordInputItem>
            {
                Getter = (context, item) => DateTime.Now,
                Setter = (context, item, value) =>
                {
                    return DavStatusCode.Ok;
                }
            },
            new DavGetResourceType<JboxSharedPasswordInputItem>
            {
                Getter = (context, item) => null
            },

            // Default locking property handling via the LockingManager
            //new DavLockDiscoveryDefault<JboxStoreItem>(),
            new DavSupportedLockDefault<JboxSharedPasswordInputItem>(),

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
            //new Win32FileAttributes<JboxStaticTxtItem>
            //{
            //    Getter = (context, item) => FileAttributes.ReadOnly,
            //    Setter = (context, item, value) =>
            //    {
            //        return DavStatusCode.Ok;
            //    }
            //}
        });

        public bool IsWritable { get; }
        public string Name => _name;
        public string UniqueKey => _name;
        public string FullPath => UriHelper.Combine(_path, _name);
        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext) => Task.FromResult((Stream)new MemoryStream(_content));

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, long start, long end) => Task.FromResult((Stream)new SubStream(new MemoryStream(_content), start, end));

        public IPropertyManager PropertyManager => DefaultPropertyManager;
        public ILockingManager LockingManager { get; }

        public async Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext)
        {
            return new StoreItemResult(DavStatusCode.Forbidden);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is JboxSharedPasswordInputItem item))
                return false;
            return item._path.Equals(_path, StringComparison.CurrentCultureIgnoreCase);
        }

        private string DetermineContentType()
        {
            return "doc/.txt";
        }

        private string CalculateEtag()
        {
            return "\"" + GetHashCode() + "\"";
        }
    }
}
