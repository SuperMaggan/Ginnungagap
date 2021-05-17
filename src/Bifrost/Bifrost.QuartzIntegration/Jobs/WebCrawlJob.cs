using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.Web;
using Bifrost.Common.Core.ApplicationServices;
using Quartz;

namespace Bifrost.QuartzIntegration.Jobs
{
    public class WebCrawlJob : ConnectorJobBase<WebConnectorJobConfiguration>
    {
        private readonly IWebConnector _webConnector;

        public WebCrawlJob( IIntegrationCoordinatorService integrationCoordinatorService, IJobService jobService, IWebConnector webConnector) 
            : base( integrationCoordinatorService, jobService)
        {
            _webConnector = webConnector;
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            var jobName = GetJobName(context);
            var configuration = GetConfiguration(jobName);
            var sourceChanges = _webConnector.ExecuteFetch(configuration);
            
            HandleFetchedDocuments(jobName, sourceChanges);
        }
     
    }
}
    