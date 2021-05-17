using System.Diagnostics;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Connectors;
using Bifrost.Core.Connectors.Configs;
using Bifrost.Core.Connectors.Configs.SqlDatabase;
using Bifrost.Common.Core.ApplicationServices;
using Quartz;
using Serilog;

namespace Bifrost.QuartzIntegration.Jobs
{
    public class SqlDatabaseConnectorJob : ConnectorJobBase<SqlDatabaseConnectorJobConfiguration>
    {
        private readonly ISqlDatabaseConnector _databaseConnectorService;
        
        public SqlDatabaseConnectorJob( IIntegrationCoordinatorService integrationCoordinatorService, IJobService jobService, ISqlDatabaseConnector databaseConnectorService)
            : base(integrationCoordinatorService, jobService)
        {
            _databaseConnectorService = databaseConnectorService;
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            var timer = new Stopwatch();
            timer.Start();

            var jobName = GetJobName(context);
            var configuration = GetConfiguration(jobName);
            var sourceChanges = _databaseConnectorService.ExecuteFetch(configuration);
            HandleFetchedDocuments(jobName, sourceChanges);

            timer.Stop();
            Log.Information($"{jobName}: Finished job in {timer.Elapsed.TotalSeconds}s");
        }

    }
}
