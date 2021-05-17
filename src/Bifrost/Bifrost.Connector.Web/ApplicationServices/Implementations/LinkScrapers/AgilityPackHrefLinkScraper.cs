using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Connector.Web.Helpers;
using HtmlAgilityPack;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations.LinkScrapers
{
    public class AgilityPackHrefLinkScraper : ILinkScraper
    {
        public IList<Uri> ScrapeLinks(Page page)
        {
            var webpage = page as WebPage;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(webpage.Html);

            var anchors = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if(anchors == null)
                return new List<Uri>();
            var hrefs = anchors.Select(x =>
                                           {
                                               var href = x.Attributes["href"];
                                               if (href != null)
                                                   return x.Attributes["href"].Value;
                                               return "";
                                           });

            
            var foundLinks = hrefs.Distinct()
                .Where(IsValidUrl)
                .Select(x=>x.ToAbsoluteUrl(page.Url.AbsoluteUri))                
                .ToList();
            return foundLinks;
        }

        public bool CanHandle(Page page)
        {

            var webPage = page as WebPage;
            return !string.IsNullOrEmpty(webPage?.Html);
        }


        private bool IsValidUrl(string url)
        {
            if (url.Contains("#") || url.Contains(":"))
                return false;

            return true;
        }


    }
}
