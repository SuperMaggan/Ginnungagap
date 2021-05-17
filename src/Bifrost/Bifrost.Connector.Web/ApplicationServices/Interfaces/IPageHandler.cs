using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;

namespace Bifrost.Connector.Web.ApplicationServices.Interfaces
{
    /// <summary>
    /// Provides an interface for impl that is responsible for handling a page's response (eg. a binary page, html page etc)
    /// </summary>
    public interface IPageHandler
    {
        /// <summary>
        /// Downloads the page from the given url
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        Page Extract(HttpResponseObject httpResponse);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        bool CanHandle(HttpResponseObject httpResponse);
    }
}