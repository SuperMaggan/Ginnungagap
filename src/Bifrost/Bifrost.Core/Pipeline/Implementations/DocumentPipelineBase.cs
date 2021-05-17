using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Domain.Document;
using Bifrost.Core.Pipeline.Interfaces;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.Pipeline.Implementations
{
    public abstract class DocumentPipelineBase : IDocumentPipeline
    {
        protected readonly IDocumentPipelineStep[] DocumentPipelineSteps;
        protected readonly IDocumentBatchPipelineStep[] DocumentBatchPipelineSteps;

        protected DocumentPipelineBase()
        {
            Log.Information(string.Format("Adding {0} per-document steps: \r\n\t{1}",
                DocumentPipelineSteps.Length,
                string.Join("\r\n\t", DocumentPipelineSteps.Select(x => x.GetType().FullName))));
            Log.Information(string.Format("Adding {0} per-document-batch steps: \r\n\t{1}",
                DocumentBatchPipelineSteps.Length,
                string.Join("\r\n\t", DocumentBatchPipelineSteps.Select(x => x.GetType().FullName))));
        }
     
        protected DocumentPipelineBase(IDocumentPipelineStep[] documentPipelineSteps, IDocumentBatchPipelineStep[] documentBatchPipelineSteps)
        {
            DocumentPipelineSteps = documentPipelineSteps;
            DocumentBatchPipelineSteps = documentBatchPipelineSteps;


        }

        public abstract IList<IDocument> InvokeSteps(IList<IDocument> documents, string domain);
    }
}