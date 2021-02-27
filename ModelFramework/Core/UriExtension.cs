using System;
using System.Linq;

namespace Kamu.ModelFramework
{
    public static class UriExtension
    {
        public static Uri Scheme(this Uri uri, string scheme) => new UriBuilder(uri) { Scheme = scheme }.Uri;

        public static Uri Provider(this Uri uri) => new UriBuilder(uri){ Query = null }.Uri;

        public static Uri Model(this Uri uri, string query) => new UriBuilder(uri){ Query = query }.Uri;

        public static string Model(this Uri uri) => new UriBuilder(uri).Query.TrimStart('?');

        public static bool AreSameLocation(this Uri uri, params Uri[] uris) 
        {
            var location = uri.Provider().Scheme("kamu");
            return uris.Select(u => u.Provider().Scheme("kamu")).All(u => u.Equals(location));
        }
    }
}
