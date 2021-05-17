using System;
using System.Collections.Generic;
using Bifrost.Connector.Web.Domain.Pages;

namespace Bifrost.Connector.Web.ApplicationServices.Interfaces
{
    public interface ILinkScraper
    {
        IList<Uri> ScrapeLinks(Page page);
        bool CanHandle(Page page);
    }

    
}