using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.Implementations
{
    /// <summary>
    /// Invokes all 
    /// </summary>
    public class SerialDocumentPipeline : DocumentPipelineBase
    {
        public SerialDocumentPipeline(IDocumentPipelineStep[] documentPipelineSteps,IDocumentBatchPipelineStep[] documentBatchPipelineSteps)
            :base(documentPipelineSteps,documentBatchPipelineSteps)
        {
            Log.Information("Initializing SerialDocumentPipeline");
        }

        /// <summary>
        /// The returned document sequence will be in the same order as the one sent in
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="domain"> </param>
        /// <returns></returns>
        public override IList<IDocument> InvokeSteps(IList<IDocument> documents, string domain)
        {
            var processedDocuments = documents
                .Select(document => InvokeAllDocumentSteps(document, domain))
                .ToList();
            return InvokeAllDocumentBatchSteps(processedDocuments, domain).ToList();
        }

        private IDocument InvokeAllDocumentSteps(IDocument document, string domain)
        {
            if (DocumentPipelineSteps.Length == 0)
                return document;

            foreach (var documentPipelineStep in DocumentPipelineSteps)
            {
                document = documentPipelineStep.Invoke(document, domain);
            }
            return document;
        }

        private IList<IDocument> InvokeAllDocumentBatchSteps(IList<IDocument> documentBatch, string domain)
        {
            if(DocumentBatchPipelineSteps.Length == 0)
                return documentBatch;
            return DocumentBatchPipelineSteps
                .Aggregate(documentBatch, (current, documentBatchStep) => documentBatchStep.Invoke(current,domain).ToList());
        }

    }
}