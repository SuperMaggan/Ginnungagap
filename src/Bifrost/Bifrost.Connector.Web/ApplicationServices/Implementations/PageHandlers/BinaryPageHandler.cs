using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
{
    public class BinaryPageHandler : PageHandlerBase, IPageHandler
    {

        public Page Extract(HttpResponseObject httpResponse)
        {
            var id = GetPageId(httpResponse.RequestUrl, httpResponse.Headers);
            if (httpResponse.ConfigUsed.PageFilter.ExcludeBinaryPages)
                return new IgnoredPage(id, $"Binary page of content type '{httpResponse.ContentType}' (PageFilter.ExcludeBinaryPages is enabled)")
                {
                    Url = httpResponse.RequestUrl,
                    ContentType = httpResponse.ContentType
                };
            return new BinaryPage(id)
            {
                Url = httpResponse.RequestUrl,
                ContentType = httpResponse.ContentType,
                Data = httpResponse.Response,
                VerifyDate = GetVerifyDate(httpResponse.Headers, httpResponse.ConfigUsed)
            };
        }

        public bool CanHandle(HttpResponseObject httpResponse)
        {
            return StatusCodeIsGreen(httpResponse.StatusCode) && httpResponse.ContentType.Contains("application/");
        }
    }
}