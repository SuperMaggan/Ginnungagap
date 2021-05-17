using System.Collections.Generic;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.Pipeline.Interfaces
{
    /// <summary>
    /// Passes documents through all registred (via constructor parameter) pipeline steps 
    /// </summary>
    public interface IDocumentPipeline
    {
        /// <summary>
        /// Will invoke all pipeline steps for each document and return the resulting document(s) 
        /// in the same order
        /// </summary>
        /// <param name="documents">A sequence of documents</param>
        /// <param name="domain">Domain that the documents belongs to</param>
        /// <returns></returns>
        IList<IDocument> InvokeSteps(IList<IDocument> documents, string domain);
    }
}