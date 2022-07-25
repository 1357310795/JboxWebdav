using System;

namespace NWebDav.Server.Helpers
{
    public static class UriHelper
    {
        public static string Combine(string baseUri, string path)
        {
            var uriText = baseUri;
            if (uriText.EndsWith("/"))
                uriText = uriText.Substring(0, uriText.Length - 1);
            return $"{uriText}/{path}";
        }

        public static Uri Combine(Uri baseUri, string path, bool isdir = false)
        {
            var uriText = baseUri.OriginalString;
            if (uriText.EndsWith("/"))
                uriText = uriText.Substring(0, uriText.Length - 1);
            if (isdir)
                return new Uri($"{uriText}/{path}/", UriKind.Absolute);
            else
                return new Uri($"{uriText}/{path}", UriKind.Absolute);
        }

        public static string ToEncodedString(Uri entryUri)
        {
            return entryUri
                .AbsoluteUri
                .Replace("#", "%23")
                .Replace("[", "%5B")
                .Replace("]", "%5D");
        }

        public static string ToEncodedString(string entryUri)
        {
            return entryUri
                .Replace("#", "%23")
                .Replace("[", "%5B")
                .Replace("]", "%5D");
        }

        public static string GetDecodedPath(Uri uri)
        {
            return uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
        }

        public static string GetPathFromUri(Uri uri)
        {
            // Determine the path
            var requestedPath = UriHelper.GetDecodedPath(uri);

            // Return the combined path
            return requestedPath;
        }

        public static string GetTopFolderFromUri(Uri uri)
        {
            var folders = uri.LocalPath.Split('/');
            if (folders.Length >= 2)
                return folders[1];
            else
                return null;
        }

        public static string GetTopFolderFromUri(string uri)
        {
            var folders = uri.Split('/');
            if (folders.Length >= 2)
                return folders[1];
            else
                return null;
        }

        public static string RemoveTopFolder(string path)
        {
            var folders = path.Split('/').ToList();
            if (folders.Count >= 2)
                folders.RemoveAt(1);
            return String.Join('/', folders);
        }
    }
}
