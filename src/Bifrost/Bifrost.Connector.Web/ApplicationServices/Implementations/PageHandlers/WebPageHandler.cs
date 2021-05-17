using System.Linq;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
{
    public class WebPageHandler : PageHandlerBase, IPageHandler
    {
        private static readonly string[] ValidContentTypes = {
            "text/",
            "application/octet-stream"
        };

        public Page Extract(HttpResponseObject httpResponse)
        {
            var html = httpResponse.Encoding.GetString(httpResponse.Response);
            return new WebPage(GetPageId(httpResponse.RequestUrl, httpResponse.Headers))
            {
                Url = httpResponse.RequestUrl,
                Html = html,
                ContentType = httpResponse.ContentType,
                VerifyDate = GetVerifyDate(httpResponse.Headers,httpResponse.ConfigUsed)
            };
        }

        public bool CanHandle(HttpResponseObject httpResponse)
        {
            return StatusCodeIsGreen(httpResponse.StatusCode) &&
                   ValidContentTypes.Any(valid => httpResponse.ContentType.Contains(valid));
        }
    }
}
