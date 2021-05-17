using System.Collections.Generic;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Pipeline.Interfaces
{
    /// <summary>
    /// Represents a step in the Document pipeline
    /// This is supposed to be used for operatiions that is invoked on a set of documents
    /// Typical operations that needs to be batched in order to be performant.
    /// </summary>
    public interface IDocumentBatchPipelineStep
    {
        /// <summary>
        /// Invoke the step logic on the given document batch 
        /// </summary>
        /// <param name="documentBatch"></param>
        /// <param name="domain"></param>
        /// <returns>The modified document batch in the same order as supplied</returns>
        IList<IDocument> Invoke(IList<IDocument> documentBatch, string domain);
    }
}