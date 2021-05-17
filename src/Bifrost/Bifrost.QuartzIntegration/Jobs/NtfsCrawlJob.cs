using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Common.Core.ApplicationServices;
using Quartz;

namespace Bifrost.QuartzIntegration.Jobs
{
    public class NtfsCrawlJob : ConnectorJobBase<FileConnectorJobConfiguration>
    {

        private readonly IFileConnector _fileConnector;

        public NtfsCrawlJob(IIntegrationCoordinatorService integrationCoordinatorService, IJobService jobService, IFileConnector fileConnector)
            : base(integrationCoordinatorService, jobService)
        {
            _fileConnector = fileConnector;
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            var jobName = GetJobName(context);
            var configuration = GetConfiguration(jobName);

            var sourceChanges = _fileConnector.ExecuteFetch(configuration);
            HandleFetchedDocuments(jobName, sourceChanges);
        }




    }





}
