using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.Implementations
{
    /// <summary>
    /// Handles all invocation in a parallell manner   
    /// </summary>
    public class ParallellDocumentPipeline : DocumentPipelineBase
    {
        public ParallellDocumentPipeline(IDocumentPipelineStep[] documentPipelineSteps, IDocumentBatchPipelineStep[] documentBatchPipelineSteps) 
            : base(documentPipelineSteps, documentBatchPipelineSteps)
        {
            Log.Information("Initializing MirroredDocumentPipeline");
        }

        /// <summary>
        /// The returned document sequence will be unordered
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="domain"> </param>
        /// <returns></returns>
        public override IList<IDocument> InvokeSteps(IList<IDocument> documents, string domain)
        {
            if (DocumentBatchPipelineSteps != null && DocumentBatchPipelineSteps.Length > 0)
                documents = InvokeAllDocumentBatchSteps(documents.ToList(), domain);

            
            var result = new ConcurrentBag<IDocument>();
            Parallel.ForEach(documents, x =>
            {
                result.Add(InvokeAllSteps(x, domain));
            });
            return result.ToList();
        }

        private IDocument InvokeAllSteps(IDocument document, string domain)
        {
            if (DocumentPipelineSteps.Length == 0)
                return document;


            return DocumentPipelineSteps
                .Aggregate(document, (current, documentPipelineStep) => documentPipelineStep.Invoke(current,domain));
        }

        private IList<IDocument> InvokeAllDocumentBatchSteps(IList<IDocument> documentBatch, string domain)
        {
            return DocumentBatchPipelineSteps
                .Aggregate(documentBatch, (current, documentBatchStep) => 
                    documentBatchStep.Invoke(current, domain).ToList());
        }
    }
}