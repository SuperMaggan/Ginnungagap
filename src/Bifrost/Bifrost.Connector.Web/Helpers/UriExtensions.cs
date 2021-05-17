using System;
using System.IO;
using System.Linq;

namespace Bifrost.Connector.Web.Helpers
{
    public static class UriExtensions
    {
        public static Uri ToAbsoluteUrl(this string url, string baseAddress)
        {
            

                if (baseAddress == null)
                    return new Uri(url);

                Uri absoluteUrl;
                if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUrl))
                    return absoluteUrl;
                Uri baseUri = new Uri(baseAddress);
                return new Uri(baseUri, url);
            
        }

        /// <summary>
        /// tries to get the extension from the url (eg. pdf, xls etc)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetExtension(this Uri url)
        {   
            var lastSegment = url.Segments.LastOrDefault();
            return lastSegment == null ? "" : Path.GetExtension(lastSegment).TrimStart('.');
        }
    }
}