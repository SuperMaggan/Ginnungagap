using System.Net;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers
{
    public class NotFoundPageHandler : IPageHandler {
        public Page Extract(HttpResponseObject httpResponse)
        {
            return new NotFoundPage(httpResponse.RequestUrl.ToString())
            {
                Reason = httpResponse?.ExceptionThrown?.Message ?? "Unknown"
            };
        }

        public bool CanHandle(HttpResponseObject httpResponse)
        {
            return httpResponse.StatusCode == HttpStatusCode.NotFound;
        }
    }
}