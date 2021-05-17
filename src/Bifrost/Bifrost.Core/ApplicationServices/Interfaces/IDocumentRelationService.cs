using System.Collections.Generic;
using Bifrost.Core.Domain.Document;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    public interface IDocumentRelationService
    {
        /// <summary>
        /// Save or update the given relation
        /// </summary>
        /// <param name="documentRelation"></param>
        void SaveOrUpdate(DocumentRelation documentRelation);
        
        /// <summary>
        /// Get the relation of the given document id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        DocumentRelation Get(string documentId);

        /// <summary>
        /// Get all DocumentIds that has the given document id in their list of relations
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        IList<string> GetRelatingDocumentIds(string documentId);
    }
}