//using System;
//using System.Globalization;
//using System.Net;
//using System.Text;
//using Bifrost.Connector.Web.ApplicationServices.Interfaces;
//using Bifrost.Connector.Web.Domain;
//using Bifrost.Core.Connectors.Configs.Web;

//namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
//{
//    public class WebClientUriHandler : IUriHandler
//    {

//        public Page Download(Uri url, WebConnectorJobConfiguration config, NetworkCredential credential = null)
//        {
//            using (var client = new WebClient())
//            {
//                try
//                {
//                    client.Credentials = credential;
//                    var response = client.DownloadData(url);
//                    var headers = client.ResponseHeaders;
//                    var contentType = headers.Get(HttpKnownHeaderNames.ContentType);


//                    if (contentType.Contains("text/html") || contentType.Contains("application/octet-stream"))
//                    {
//                        var html = Encoding.UTF8.GetString(response);
//                        return new WebPage(GetPageId(url, headers))
//                        {
//                            Url = url,
//                            Html = html,
//                            ContentType = contentType,
//                            VerifyDate = GetVerifyDate(config, headers)
//                        };
//                    }
//                    if (config.PageFilter.ExcludeBinaryPages)
//                        return new IgnoredPage(GetPageId(url, headers), $"Binary page of content type '{contentType}' (PageFilter.ExcludeBinaryPages is enabled)")
//                        {
//                            Url = url,
//                            ContentType = contentType
//                        };
//                    return new BinaryPage(GetPageId(url, headers))
//                    {
//                        Url = url,
//                        ContentType = contentType,
//                        Data = response,
//                        VerifyDate = GetVerifyDate(config, headers)
//                    };
//                }
//                catch (WebException e)
//                {
//                    if (e.Status == WebExceptionStatus.ProtocolError)
//                    {
//                        return new NotAuthorizedPage(GetPageId(url, e.Response.Headers))
//                        {
//                            Url = url,
//                            Status = (int)(e.Response as HttpWebResponse).StatusCode,
//                            Reason = e.Message
//                        };
//                    }
//                    return new NotFoundPage(GetPageId(url, e.Response.Headers))
//                    {
//                        Url = url,
//                        Reason = e.Message
//                    };
//                }
//                catch (Exception e)
//                {
//                    return new NotFoundPage(GetPageId(url))
//                    {
//                        Url = url,
//                        Reason = e.Message
//                    };
//                }
//            }

//        }

//        private string GetPageId(Uri url, WebHeaderCollection headers = null)
//        {
//            //Example: Link:</node/6>; rel="shortlink",</home>; rel="canonical"

//            var linkHead = headers?.Get("Link") ?? "";
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

//        private DateTime GetVerifyDate(WebConnectorJobConfiguration config, WebHeaderCollection headers)
//        {
//            //Example: Expires:Sun, 19 Nov 1978 05:00:00 GMT
//            var expiresHead = headers.Get("Expires");
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