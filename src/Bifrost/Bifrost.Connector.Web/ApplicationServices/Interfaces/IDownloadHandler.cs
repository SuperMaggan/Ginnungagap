using System;
using System.Net;
using Bifrost.Connector.Web.Domain;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;

namespace Bifrost.Connector.Web.ApplicationServices.Interfaces
{
    public interface IDownloadHandler
    {
        /// <summary>
        /// Downloads the page from the given url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="config"></param>
        /// <param name="credential"></param>
        /// <returns></returns>
        HttpResponseObject Download(Uri url, WebConnectorJobConfiguration config, NetworkCredential credential = null);
        
    }
}