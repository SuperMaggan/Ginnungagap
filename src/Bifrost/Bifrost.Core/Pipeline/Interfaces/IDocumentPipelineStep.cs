using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Pipeline.Interfaces
{
    /// <summary>
    /// Represents a step in the Document pipeline
    /// Contains transformation/enrichment logic to invoke on Documents
    /// </summary>
    public interface IDocumentPipelineStep
    {
        /// <summary>
        /// Invoke the step rule on the given document
        /// </summary>
        /// <param name="document"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        IDocument Invoke(IDocument document, string domain);

        bool Enabled { get; set; }
    }
}