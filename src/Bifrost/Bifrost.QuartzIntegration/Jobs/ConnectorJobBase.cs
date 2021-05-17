using System;
using System.Linq;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.QuartzIntegration.Jobs;
using Serilog;

namespace Bifrost.QuartzIntegration.Jobs
{
    public abstract class ConnectorJobBase<T> : ConfigurableJobBase<T>
        where T : class, IJobConfiguration
    {
        protected readonly IIntegrationCoordinatorService IntegrationCoordinatorService;

        protected ConnectorJobBase(IIntegrationCoordinatorService integrationCoordinatorService, IJobService jobService) 
            : base(jobService)
        {
            IntegrationCoordinatorService = integrationCoordinatorService;
        }

        protected bool IntegrationIsReady(string sourceName)
        {
            if (!IntegrationCoordinatorService.ReadyForDocuments())
            {
                Log.Warning($"{sourceName}: Will not fetch documents. {IntegrationCoordinatorService.GetType().Name} is not ready");
                return false;
            }
            return true;
        }

        protected void HandleFetchedDocuments(string jobName, SourceChanges changes)
        {
            if (!changes.Adds.Any() && !changes.Deletes.Any())
                return;
            Log.Information($"{jobName}: Fetched {changes.Adds.Count} ADDS and {changes.Deletes.Count} DELETES");

            if (changes.Adds != null && changes.Adds.Any())
            {
                var dateString = DateTime.Now.ToString();
                foreach (var document in changes.Adds)
                {
                    document.Id = GetDocumentId(jobName, document.Id);
                    document.Fields.Add(new Field("Connector-JobName", jobName));
                    document.Fields.Add(new Field("Connector-FetchDate", dateString));
                }
            }
            if (changes.Deletes != null && changes.Deletes.Any())
            {
                foreach (var delete in changes.Deletes)
                {
                    delete.Id= GetDocumentId(jobName, delete.Id);
                }
            }

            IntegrationCoordinatorService.HandleDocuments(
                changes.Adds.Union<IDocument>(changes.Deletes).ToList()
                , jobName);

        }

        
        private string GetDocumentId(string jobName, string id)
        {
            return $"{jobName}_{id}";
        }

    }
}