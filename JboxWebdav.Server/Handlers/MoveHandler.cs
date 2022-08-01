using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using JboxWebdav.Server.Jbox;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace NWebDav.Server.Handlers
{
    /// <summary>
    /// Implementation of the MOVE method.
    /// </summary>
    /// <remarks>
    /// The specification of the WebDAV MOVE method can be found in the
    /// <see href="http://www.webdav.org/specs/rfc2518.html#METHOD_MOVE">
    /// WebDAV specification
    /// </see>.
    /// </remarks>
    public class MoveHandler : IRequestHandler
    {
        /// <summary>
        /// Handle a MOVE request.
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context of the request.
        /// </param>
        /// <param name="store">
        /// Store that is used to access the collections and items.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous MOVE operation. The task
        /// will always return <see langword="true"/> upon completion.
        /// </returns>
        public async Task<bool> HandleRequestAsync(IHttpContext httpContext, IStore store)
        {
            // Obtain request and response
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (!Config.AccessMode.CheckAccess(JboxAccessMode.move))
            {
                response.SetStatus(DavStatusCode.Forbidden);
                return true;
            }

            // We should always move the item from a parent container
            var splitSourceUri = RequestHelper.SplitUri(request.Url);

            // Obtain source collection
            var sourceCollectionUri = UriHelper.GetPathFromUri(splitSourceUri.CollectionUri);
            var sourceItemUri = UriHelper.Combine(sourceCollectionUri, splitSourceUri.Name);

            // Obtain the destination
            var destinationUri = request.GetDestinationUri();
            if (destinationUri == null)
            {
                // Bad request
                response.SetStatus(DavStatusCode.BadRequest, "Destination header is missing.");
                return true;
            }

            // Make sure the source and destination are different
            if (request.Url.AbsoluteUri.Equals(destinationUri.AbsoluteUri, StringComparison.CurrentCultureIgnoreCase))
            {
                // Forbidden
                response.SetStatus(DavStatusCode.Forbidden, "Source and destination cannot be the same.");
                return true;
            }

            // We should always move the item to a parent
            var splitDestinationUri = RequestHelper.SplitUri(destinationUri);

            // Obtain destination collection
            var destinationCollectionUri = UriHelper.GetPathFromUri(splitDestinationUri.CollectionUri);
            var destinationItemUri = UriHelper.Combine(destinationCollectionUri, splitDestinationUri.Name);

            var topfolder = UriHelper.GetTopFolderFromUri(sourceItemUri);
            if (topfolder == "他人的分享链接")
            {
                var sourceCollection = await store.GetCollectionAsync(splitSourceUri.CollectionUri, httpContext).ConfigureAwait(false);
                var destinationCollection = await store.GetCollectionAsync(splitDestinationUri.CollectionUri, httpContext).ConfigureAwait(false);
                var result = await sourceCollection.MoveItemAsync(splitSourceUri.Name, destinationCollection, splitDestinationUri.Name, true, httpContext).ConfigureAwait(false);

                response.SetStatus(result.Result);
                return true;
            }

            if (topfolder == "交大空间")
            {
                var sourceCollection = await store.GetCollectionAsync(splitSourceUri.CollectionUri, httpContext).ConfigureAwait(false);
                var destinationCollection = await store.GetCollectionAsync(splitDestinationUri.CollectionUri, httpContext).ConfigureAwait(false);
                var result = await sourceCollection.MoveItemAsync(splitSourceUri.Name, destinationCollection, splitDestinationUri.Name, true, httpContext).ConfigureAwait(false);

                response.SetStatus(result.Result);
                return true;
            }

            JboxMoveItemInfo res = null;
            if (sourceCollectionUri == destinationCollectionUri)
            {
                res = JboxService.RenameJboxItem(sourceItemUri, destinationItemUri);
            }
            else
            {
                res = JboxService.MoveJboxItem(sourceItemUri, destinationCollectionUri);
            }

            if (res.success)
            {
                // Set the response
                response.SetStatus(DavStatusCode.Ok);
                return true;
            }
            
            if (res.Code == "source not found")
            {
                // Source not found
                response.SetStatus(DavStatusCode.NotFound);
                return true;
            }
            
            if (res.Code == "target name exists")
            {
                // Cannot overwrite destination item
                response.SetStatus(DavStatusCode.PreconditionFailed, "Cannot overwrite destination item.");
                return true;
            }

            response.SetStatus(DavStatusCode.BadRequest);
            return true;
        }

        //private async Task MoveAsync(IStoreCollection sourceCollection, IStoreItem moveItem, IStoreCollection destinationCollection, string destinationName, bool overwrite, IHttpContext httpContext, Uri baseUri, UriResultCollection errors)
        //{
        //    // Determine the new base URI
        //    var subBaseUri = UriHelper.Combine(baseUri, destinationName);

        //    // Obtain the actual item
        //    if (moveItem is IStoreCollection moveCollection && !moveCollection.SupportsFastMove(destinationCollection, destinationName, overwrite, httpContext))
        //    {
        //        // Create a new collection
        //        var newCollectionResult = await destinationCollection.CreateCollectionAsync(destinationName, overwrite, httpContext).ConfigureAwait(false);
        //        if (newCollectionResult.Result != DavStatusCode.Created && newCollectionResult.Result != DavStatusCode.NoContent)
        //        {
        //            errors.AddResult(subBaseUri, newCollectionResult.Result);
        //            return;
        //        }

        //        // Move all sub items
        //        foreach (var entry in await moveCollection.GetItemsAsync(httpContext).ConfigureAwait(false))
        //            await MoveAsync(moveCollection, entry, newCollectionResult.Collection, entry.Name, overwrite, httpContext, subBaseUri, errors).ConfigureAwait(false);

        //        // Delete the source collection
        //        var deleteResult = await sourceCollection.DeleteItemAsync(moveItem.Name, httpContext).ConfigureAwait(false);
        //        if (deleteResult != DavStatusCode.Ok)
        //            errors.AddResult(subBaseUri, newCollectionResult.Result);
        //    }
        //    else
        //    {
        //        // Items should be moved directly
        //        var result = await sourceCollection.MoveItemAsync(moveItem.Name, destinationCollection, destinationName, overwrite, httpContext).ConfigureAwait(false);
        //        if (result.Result != DavStatusCode.Created && result.Result != DavStatusCode.NoContent)
        //            errors.AddResult(subBaseUri, result.Result);
        //    }
        //}

        //private async Task MoveAsync(string sourceCollectionUri, string destinationCollectionUri, string destinationName, IHttpContext httpContext)
        //{
        //    // Determine the new base URI
        //    var subBaseUri = UriHelper.Combine(baseUri, destinationName);

        //    // Items should be moved directly
        //    var result = await sourceCollection.MoveItemAsync(moveItem.Name, destinationCollection, destinationName, overwrite, httpContext).ConfigureAwait(false);
        //    if (result.Result != DavStatusCode.Created && result.Result != DavStatusCode.NoContent)
        //        errors.AddResult(subBaseUri, result.Result);
        //}
    }
}
