using System.Collections.Generic;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    /// <summary>
    /// Handles add and delete documents
    /// </summary>
    public interface IIntegrationCoordinatorService
    {
        /// <summary>
        /// Feed the pipe with documents to add
        /// </summary>
        /// <param name="documents">A sequence of documents</param>
        /// <param name="domain"></param>
        void HandleDocuments(IList<IDocument> documents, string domain);
        
        /// <summary>
        /// Validates if all Integration services are ready to process documents
        /// </summary>
        /// <returns>True if all integration points are ready, false if one or more experiencing issues</returns>
        bool ReadyForDocuments();
    }
}
    
