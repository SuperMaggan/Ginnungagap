using System.Net;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
{
    public class NotAuthorizedPageHandler : PageHandlerBase, IPageHandler
    {
        public Page Extract(HttpResponseObject httpResponse)
        {
            return new NotAuthorizedPage(GetPageId(httpResponse.RequestUrl, httpResponse.Headers))
            {
                Url = httpResponse.RequestUrl,
                Status = (int)httpResponse.StatusCode,
                Reason = httpResponse.ExceptionThrown?.Message,
            };
        }

        public bool CanHandle(HttpResponseObject httpResponse)
        {
            return httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.Forbidden;
        }
    }
}