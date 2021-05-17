using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Connector.Web.Domain;
using Bifrost.Connector.Web.Domain.Pages;
using Bifrost.Connector.Web.Helpers;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Core.Connectors.States.WebCrawler;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;
using Serilog;
using JobState = Bifrost.Core.Connectors.States.JobState;

namespace Bifrost.Connector.Web
{
    public class WebCrawlerConnector : IWebConnector
    {
        private readonly IQueueService _queueService;
        private readonly IDocumentStateService _documentStateService;
        private readonly IPageService _pageService;
        private readonly IStateService _stateService;

        public WebCrawlerConnector(IQueueService queueService, IDocumentStateService documentStateService, IPageService pageService, IStateService stateService)
        {
            _queueService = queueService;
            _documentStateService = documentStateService;
            _pageService = pageService;
            _stateService = stateService;
        }


        public SourceChanges ExecuteFetch(WebConnectorJobConfiguration config)
        {
            var timer = new Stopwatch(); timer.Start();
            var state = GetCurrentJobState(config);
            state.LastExecutionDate = DateTime.UtcNow;

            var queue = _queueService.Pop(config.JobName, config.NumberOfPagesPerExecution).ToPageQueueItems();
            try
            {
                if (!queue.Any() && state.State == JobState.InitialCrawling)
                {
                    var changes = InitialCrawl(config, state);
                    state.State = JobState.IncrementalCrawling;
                    return changes;
                }
                if (!queue.Any())
                {
                    var docsToVerify = _documentStateService.PushVerifyDocumentsToQueue(config.JobName);
                    Log.Information($"{config.JobName}: {docsToVerify} documents that should be verified pushed to queue");
                    if (docsToVerify > 0)
                        queue = _queueService.Pop(config.JobName, config.NumberOfPagesPerExecution).ToPageQueueItems();
                }

                state.Status = JobStatus.Ok;
                return CrawlLinks(config, queue, state);

            }
            catch (Exception e)
            {
                var error = $"{config.JobName}: Error when crawling, {e.Message}. Pushing {queue.Count} queue items back to stack";
                Log.Error(e,error);
                state.SetErrorState(e);
                _queueService.Push(config.JobName, queue.ToQueueItems());
                state.BatchCount = _queueService.Count(config.JobName);
                return new SourceChanges();
            }
            finally
            {
                timer.Stop();
                state.LastExecutionTimeMs = (int) timer.ElapsedMilliseconds;
                _stateService.SaveState(state);
            }

        }

        public void ResetConnector(string jobName)
        {
            _queueService.Delete(jobName);
            _documentStateService.Delete(jobName);
            Log.Information($"{jobName}: Reset the webcrawler job");
        }


        private SourceChanges InitialCrawl(WebConnectorJobConfiguration config, WebCrawlerJobState state)
        {

            return CrawlLinks(config,
                       new List<PageQueueItem>()
                           {
                               new PageQueueItem()
                                   {
                                       CreateDate = DateTime.UtcNow,
                                       Id= config.StartUrl,
                                       Url= new Uri(config.StartUrl)
                                   }
                           }, state);
        }


        /// <summary>
        /// Handles the queueitems (links) given
        /// Checks if the links should be downloaded
        /// </summary>
        /// <param name="config"></param>
        /// <param name="queue"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private SourceChanges CrawlLinks(WebConnectorJobConfiguration config, IList<PageQueueItem> queue, WebCrawlerJobState state)
        {
            if(!queue.Any())
                return new SourceChanges();

            var documents = new List<IDocument>(queue.Count);
            var info = new StringBuilder();
            foreach (var queueItem in queue)
            {
                Log.Information($"Crawling {queueItem.Id}");
                var pageState = _documentStateService.Get(config.JobName, queueItem.Id).ToPageState();
                if (!pageState.ShouldVerify())
                {
                    info.AppendLine($"{queueItem.Id} (Skipped since it already exists, will be verified {pageState.VerifyDate})");
                    continue;
                }
                
                var page = _pageService.Download(queueItem.Url, config);
                if (page.Id != queueItem.Id)//the page uses eg. a canonical url, check for a new DocumentState for that id
                {
                    pageState = _documentStateService.Get(config.JobName, page.Id).ToPageState();
                    queueItem.Depth = pageState?.Depth ?? queueItem.Depth;
                }
                page.Depth = queueItem.Depth;

                documents.Add(HandlePage(page,config,pageState,info));
            }

            var queueCount= _queueService.Count(config.JobName);
            state.BatchCount = queueCount;
            state.Message = $"Handled {queue.Count} pages ({queueCount} left in queue): \r\n{info}";
            return new SourceChanges(documents);
        }

     
        private int HandleLinks(Page page, WebConnectorJobConfiguration config)
        {
            if (page.Depth >= config.Depth) {
                Log.Information($"{config.JobName}: {page.Url} is at depth {page.Depth} which is the maximum allowed depth ({config.Depth}), links from this page will be ignored");
                return 0;
            }
            var links = _pageService.ScrapeLinks(page).ToList();
            var validLinks = _pageService.GetValidLinks(page, links, config.LinkFilter);

            var newLinks = _queueService.Push(
                config.JobName,
                validLinks.Select(link => link.ToQueueItem(page)).ToList());
            Log.Information($"{config.JobName}: Added {newLinks} new links (total valid links / total links: " +
                        $"{validLinks.Count}/{links.Count}) from {page.Url.AbsoluteUri} (depth: {page.Depth})");
            return newLinks;
        }

        /// <summary>
        /// Tries to get the current job state
        /// If no state exists, a new job state will be created and initiated
        /// </summary>
        /// <param name="config"></param>
        private WebCrawlerJobState GetCurrentJobState(WebConnectorJobConfiguration config)
        {
            var state = _stateService.LoadState(config.JobName);

            if (state == null)
            {
                Log.Information($"{config.JobName}: Initializing fresh crawl");
                ResetConnector(config.JobName);
                state = new WebCrawlerJobState()
                {
                    InitDate = DateTime.UtcNow,
                    Message = "Initializing..",
                    State = JobState.InitialCrawling,
                    LastExecutionDate = DateTime.UtcNow,
                    Name = config.JobName,
                };
                _stateService.SaveState(state);
            }
            return new WebCrawlerJobState(state);
        }


        private IDocument HandlePage(Page page, WebConnectorJobConfiguration config, PageState pageState, StringBuilder information)
        {
            if (page is IgnoredPage)
                return HandleIgnorePage(page as IgnoredPage,config, information);
            if (page is NotFoundPage)
                return HandleNotFoundPage(page as NotFoundPage, config,information);
            if (page is NotAuthorizedPage)
                return HandleNotAuthorizedPage(page as NotAuthorizedPage, config, information);
            if (page is WebPage)
                return HandleWebpage(page as WebPage, config, pageState, information);
            if (page is BinaryPage)
                return HandleBinaryPage(page as BinaryPage, config, pageState, information);
            throw new NotSupportedException("No support for handling page of type " + page.GetType().Name);

        }

        private IDocument HandleIgnorePage(IgnoredPage page, WebConnectorJobConfiguration config, StringBuilder information)
        {
            information.AppendLine($"{page.Id} (Page is ignored: {(page as IgnoredPage).Reason})");
            return new IgnoreDocument(page.Id,config.JobName, page.Reason);
        }

        private IDocument HandleNotFoundPage(NotFoundPage page, WebConnectorJobConfiguration config, StringBuilder information)
        {
            _documentStateService.Delete(config.JobName, page.Id);
            information.AppendLine($"{page.Id} (Page not found. Sending delete command)");
            return new DeleteDocument(page.Id, config.JobName) ;

        }
        private IDocument HandleNotAuthorizedPage(NotAuthorizedPage page, WebConnectorJobConfiguration config, StringBuilder information)
        {
            _documentStateService.Delete(config.JobName, page.Id);
            information.AppendLine($"{page.Id} (Page not authorized.)");
            Log.Warning($"Not authorized to see page {page.Url} ({page.Reason})");
            return new DeleteDocument(page.Id, config.JobName);
        }

        private IDocument HandleWebpage(WebPage page, WebConnectorJobConfiguration config, PageState pageState, StringBuilder information)
        {
            if (!pageState.PageIsChanged(page))
            {
                information.AppendLine($"{page.Id} (No change detected, skipping)");
                return new IgnoreDocument(page.Id, config.JobName, "No change detected, skipping");
            }

            var newLinkCount = HandleLinks(page, config);
            Log.Information($"Adding WebPage {page.Id}");
            _documentStateService.UpdatePageState(page, config.JobName);
            var linkInfo = page.Depth >= config.Depth
                    ? $"and ignoring links since the depth is {page.Depth}"
                    : $"with {newLinkCount} new links (with depth {page.Depth + 1})";
            information.AppendLine(pageState == null
                    ? $"{page.Id} (Adding new html page {linkInfo}"
                    : $"{page.Id} (Adding updated html page {linkInfo}");
            return _pageService.ConvertToDocument(page, config.JobName);
        }

        private IDocument HandleBinaryPage(BinaryPage page, WebConnectorJobConfiguration config, PageState pageState,
            StringBuilder information)
        {
            if (!pageState.PageIsChanged(page))
            {
                information.AppendLine($"{page.Id} (No change detected, skipping)");
                return new IgnoreDocument(page.Id, config.JobName, "No change detected, skipping");
            }

            Log.Information($"Adding BinaryPage {page.Id}");
            _documentStateService.UpdatePageState(page, config.JobName);
            information.AppendLine(pageState == null
                    ? $"{page.Id} (Adding new binary page"
                    : $"{page.Id} (Adding updated binary page");
            return _pageService.ConvertToDocument(page, config.JobName);
        }

    }
}