using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    /// <summary>
    /// Will send all documents and deletes to all registred integration services
    /// </summary>
    public class MirrorIntegrationCoordinatorService : IIntegrationCoordinatorService
    {
        private readonly IIntegrationService[] _integrationServices;
        private readonly IDocumentPipeline _documentPipeline;
        private readonly IDocumentCache _documentCache;

     
        public MirrorIntegrationCoordinatorService(IIntegrationService[] integrationServices, IDocumentPipeline documentPipeline, IDocumentCache documentCache)
        {
            _integrationServices = integrationServices;
            _documentPipeline = documentPipeline;
            _documentCache = documentCache;
            
            if(!_integrationServices.Any())
                Log.Warning("No integration services enabled (app.config). No dispatch will take place.");
        }

        public MirrorIntegrationCoordinatorService(IIntegrationService[] integrationServices, IDocumentPipeline documentPipeline)
            :this(integrationServices, documentPipeline, null){}


        public void HandleDocuments(IList<IDocument> documents, string domain)
        {
            if (documents == null)
                return;
            var processedDocuments = _documentPipeline.InvokeSteps(documents, domain);
            HandleDocuments(processedDocuments);
        }


        private void HandleDocuments(IList<IDocument> documents)
        {
            if(!documents.Any())
                return;

            var trySendCachedDocuments = _documentCache != null;
            var cacheDocuments = false;

            var documentsPerDomain = documents.GroupBy(x => x.Domain);
            Parallel.ForEach(_integrationServices, integrationService =>
            {
                try
                {
                    foreach (var domainDocuments in documentsPerDomain)
                    {
                        integrationService.HandleDocuments(domainDocuments.ToList());
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"{integrationService.GetType().Name} threw an error while dispatching documents: {e.Message}");
                    cacheDocuments = _documentCache != null;
                }
            });
          
            if (cacheDocuments)
            {
                Log.Information("Caching documents due to integration point failure");
                _documentCache.HandleDocuments(documents.ToList());
            }
            else if (trySendCachedDocuments)
            {
                foreach (var domain in documentsPerDomain)
                {
                    HandleDocuments(_documentCache.LoadDocumentsBatch(domain.Key, true).Cast<IDocument>().ToList());
                    HandleDocuments(_documentCache.LoadDeleteBatch(domain.Key, true).Cast<IDocument>().ToList());
                }
                
            }
        }


        public bool ReadyForDocuments()
        {
            foreach (var integrationService in _integrationServices)
            {
                if (!integrationService.CanConnect())
                {
                    Log.Warning($"{integrationService.GetType().Name} is experiencing issues.");
                    return false;
                }
            }
            return true;
        }



    }
}