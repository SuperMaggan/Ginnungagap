using Bifrost.Connector.Web;
using Bifrost.Connector.Web.ApplicationServices.Implementations;
using Bifrost.Connector.Web.ApplicationServices.Implementations.DownloadHandlers;
using Bifrost.Connector.Web.ApplicationServices.Implementations.LinkScrapers;
using Bifrost.Connector.Web.ApplicationServices.Implementations.PageConverters;
using Bifrost.Connector.Web.ApplicationServices.Implementations.PageHandlers;
using Bifrost.Connector.Web.ApplicationServices.Interfaces;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Common;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Data.Sql;
using Bifrost.Data.Sql.Databases;
using Bifrost.Data.Sql.Databases.Bifrost.Mappers;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Xunit;

namespace Bifrost.IntegrationTests.Connectors
{
    public class WebConnectorTests
    {
        [Fact]
        public void Can_Crawl_Blog()
        {
            var dbSettings = new SqlSettings();
            var database = new AsgardDatabase(dbSettings);
            var queueService = new SqlQueueService(database, new QueueItemMapper());
            var documentStateService = new SqlDocumentStateService(database, new DocumentStateMapper());

            var linkScrapers = new ILinkScraper[] {new AgilityPackHrefLinkScraper()};
            var pageConverters = new IPageConverter[] {new WebPageConverter()};
            var downloadHandler = new HttpClientDownloadHandler();
            var pageHandlers = new IPageHandler[]
            {
                new BinaryPageHandler(), new NotAuthorizedPageHandler(), new NotFoundPageHandler(), new WebPageHandler()
            };
            var pageService = new ParallelPageService(linkScrapers,pageConverters,downloadHandler,pageHandlers);

            var stateService = new SqlStateService(new StateDatabase(new CommonDatabaseSettings()
            {
                CommonConnection = dbSettings.BifrostConnection
            }));
            var connector = new WebCrawlerConnector(queueService, documentStateService, pageService, stateService);


            var fetched = connector.ExecuteFetch(new WebConnectorJobConfiguration()
            {
                StartUrl = "http://blog.cwa.me.uk",
                JobName = "Test_cwablog",
                NumberOfPagesPerExecution = 10,
                Credential = null,
                DefaultVerifyFrequency = new Frequency() { Days = 1},
                Depth = 2,
                LinkFilter = new LinkFilter() { StayOnHost = true},
                PageFilter = new PageFilter() { ExcludeBinaryPages = true}
            });

            
            
        }

        //[Fact]
        //public void CanHandleNotFound()
        //{
        //    var downloadHandler = new WebRequestDownloadHandler();

        //    var uri = new Uri("http://findwsssaadses.com");
        //    var config = new WebConnectorJobConfiguration() { DefaultVerifyFrequency = new Frequency() };
        //    var responseObject = downloadHandler.Download(uri, config);

        //    responseObject.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //}

        //[Fact]
        //public void CanHandleRedirectToOtherDomaion()
        //{
        //    var downloadHandler = new WebRequestDownloadHandler();

        //    var uri = new Uri("http://canalen/-System-/PerformanceSSO/");
        //    var config = new WebConnectorJobConfiguration() { DefaultVerifyFrequency = new Frequency() };

        //    var credentials = new NetworkCredential("psok-svc", "K4#9QemPB", "FKA");
        //    var responseObject = downloadHandler.Download(uri, config,credentials);

        //    responseObject.StatusCode.Should().Be(HttpStatusCode.ProxyAuthenticationRequired);
        //}
    }
}
