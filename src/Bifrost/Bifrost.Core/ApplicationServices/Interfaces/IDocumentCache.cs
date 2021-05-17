using System.Collections.Generic;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    public interface IDocumentCache : IIntegrationService
    {


        /// <summary>
        /// Loads the first batch of documents stored in the cache
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="removeLoadedDocuments"></param>
        /// <returns></returns>
        IEnumerable<AddDocument> LoadDocumentsBatch(string domain, bool removeLoadedDocuments);
        IEnumerable<DeleteDocument> LoadDeleteBatch(string domain, bool removeLoadedDeletes);

        /// <summary>
        /// Loads all stored documents
        /// </summary>
        /// <param name="removeLoadedDocuments">If true, the loaded documents will be delete from the cache</param>s
        IEnumerable<AddDocument> LoadAllDocuments(string domain, bool removeLoadedDocuments);

        /// <summary>
        /// Loads all stored Deletes
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="removeLoadedDeletes">If true the loaded deletes will be deleted from the cache</param>
        /// <returns></returns>
        IEnumerable<DeleteDocument> LoadAllDeletes(string domain,bool removeLoadedDeletes);

        IEnumerable<string> LoadAllDomainNames();

    }
}