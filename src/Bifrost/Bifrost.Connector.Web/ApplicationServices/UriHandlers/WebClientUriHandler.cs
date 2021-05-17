//using System;
//using System.Globalization;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Bifrost.Connector.Web.Domain.Pages;
//using Bifrost.Core.Connectors.Configs.Web;

//namespace Bifrost.Connector.Web.ApplicationServices.UriHandlers
//{
//    public class HttpClientUriHandler : IUriHandler
//    {

//        public Page Download(Uri url, WebConnectorJobConfiguration config, NetworkCredential credential = null)
//        {
//            using (var handler = new HttpClientHandler() { Credentials = credential })
//            using (var client = new HttpClient())
//            {

//                var response = client.GetAsync(url).Result;
//                if (!response.IsSuccessStatusCode)
//                {
//                    if (response.StatusCode == HttpStatusCode.Forbidden)
//                        return new NotAuthorizedPage(GetPageId(url, e.Response.Headers))
//                        {
//                            Url = url,
//                            Status = (int)response.StatusCode,
//                            Reason = response.ReasonPhrase
//                        };
//                    return new NotFoundPage(GetPageId(url, response.Headers))
//                    {
//                        Url = url,
//                        Reason = response.ReasonPhrase
//                    };
//                }


//                var headers = response.Headers;
//                var contentType = headers.GetValues("content-type");

//            }

//        }

//        private string GetPageId(Uri url, HttpResponseHeaders headers = null)
//        {
//            //Example: Link:</node/6>; rel="shortlink",</home>; rel="canonical"

//            var linkHead = headers?.GetValues("Link").FirstOrDefault() ?? "";
//            if (string.IsNullOrEmpty(linkHead))
//                return url.ToString();
//            var tokens = linkHead.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (var token in tokens)
//            {
//                var subTokens = token.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
//                if (subTokens.Length != 2)
//                    continue;
//                if (subTokens[1].Contains("canonical"))
//                    return subTokens[0].Trim('<', '>');
//            }
//            return url.ToString();
//        }

//        private DateTime GetVerifyDate(WebConnectorJobConfiguration config, HttpResponseHeaders headers)
//        {
//            //Example: Expires:Sun, 19 Nov 1978 05:00:00 GMT
//            var expiresHead = headers.GetValues("Expires").FirstOrDefault();
//            DateTime date;
//            if (DateTime.TryParse(expiresHead, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
//            {
//                if (date > DateTime.UtcNow)
//                    return date;
//            }
//            return DateTime.UtcNow + config.DefaultVerifyFrequency.ToTimeSpan();

//        }


//        public bool CanHandle(Uri url, WebConnectorJobConfiguration config)
//        {
//            return true;
//        }
//    }
//}