using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Connector.Web.Helpers;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;
using Serilog;
using Serilog.Core;

namespace Bifrost.Connector.Web.ApplicationServices.Implementations
{
    public class ParallelPageService : IPageService {

        private readonly ILinkScraper[] _linkScrapers;
        private readonly IPageConverter[] _pageConverters;
        private readonly IDownloadHandler _downloadHandler;
        private readonly IPageHandler[] _pageHandlers;

        public ParallelPageService( ILinkScraper[] linkScrapers, IPageConverter[] pageConverters, IDownloadHandler downloadHandler, IPageHandler[] pageHandlers)
        {
            _linkScrapers = linkScrapers;
            _pageConverters = pageConverters;
            _downloadHandler = downloadHandler;
            _pageHandlers = pageHandlers;
        }

       public Page Download(Uri url, WebConnectorJobConfiguration config)
       {
           var responseObject = _downloadHandler.Download(url, config, config.Credential.ToNetworkCredential());

            var pageHandler = _pageHandlers.FirstOrDefault(x => x.CanHandle(responseObject));
            return pageHandler == null 
                ? new IgnoredPage(url.ToString(), $"No Page handler registered that supports {responseObject}") 
                : pageHandler.Extract(responseObject);
       }

        public IDocument ConvertToDocument(Page page, string domain)
        {
            var pageConverter = _pageConverters.FirstOrDefault(x => x.CanHandle(page));
            if(pageConverter == null)
                throw new NotSupportedException($"No Converter registered that supports {page}");
            return pageConverter.ConvertToDocument(page, domain);

        }

        public IList<IDocument> ConvertToDocument(IList<Page> pages, string domain)
        {
            var result = new ConcurrentBag<IDocument>();
            Parallel.ForEach(pages, page =>
            {
                result.Add(ConvertToDocument(page, domain));
            });
            return result.ToList();
        }

        public IList<Uri> ScrapeLinks(Page page)
        {
            var result = new ConcurrentBag<Uri>();
            Parallel.ForEach(_linkScrapers, scraper =>
            {
                if(scraper.CanHandle(page))
                    foreach (var scrapedLink in scraper.ScrapeLinks(page))
                    {
                        result.Add(scrapedLink);
                    }
            });
            return result.ToList();
        }

        public IList<Uri> GetValidLinks(Page sourcePage, IList<Uri> links, LinkFilter filter)
        {

            return links
                .Where(link => IsValid(sourcePage, link,filter))
                .ToList();
        }

        private bool IsValid(Page sourcePage, Uri link, LinkFilter filter)
        {
            var reason = "";
            if (!filter.StayOnHost | sourcePage.Url.Host != link.Host)
            {
                Log.Debug($"Excluded: {link} (Host '{link.Host}' differs from source page's host '{sourcePage.Url.Host}')");
                return false;
            }
            var valid = filter.IsValid(link, ref reason);
            Log.Debug(valid 
                ? $"Added: {link}" 
                : $"Excluded: {link} ({reason})");
            return valid;
        }
    }
}