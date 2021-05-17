using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;

namespace Bifrost.Processing.CsScript
{
    public class CsScriptProcessingDocumentBatchStep : IDocumentBatchPipelineStep
    {
        private readonly CsScriptProcessingService _processingService;

        public CsScriptProcessingDocumentBatchStep(CsScriptProcessingService processingService)
        {
            _processingService = processingService;
        }

        public IList<DocumentBase> Invoke(IList<DocumentBase> documentBatch, string domain)
        {
            var processedDocuments = _processingService.ProcessDocuments(documentBatch, domain);
            return processedDocuments.ToList();
        }

        public override string ToString()
        {
            return string.Format("{0}: using {1} ",
                this.GetType().Name,
                _processingService);
        }
    }
    
}
