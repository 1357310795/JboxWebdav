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

        public static Uri Combine(Uri baseUri, string path)
        {
            var uriText = baseUri.OriginalString;
            if (uriText.EndsWith("/"))
                uriText = uriText.Substring(0, uriText.Length - 1);
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
    }
}
