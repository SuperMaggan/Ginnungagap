using System;
using System.Collections.Generic;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Connector.Web.ApplicationServices.Interfaces
{
    public interface IPageService
    {
        Page Download(Uri url, WebConnectorJobConfiguration config);

        IDocument ConvertToDocument(Page page, string domain);
        IList<IDocument> ConvertToDocument(IList<Page> pages, string domain);


        IList<Uri> ScrapeLinks(Page page);

        IList<Uri> GetValidLinks(Page sourcePage, IList<Uri> links, LinkFilter filter);
    }
    

    
}