using Bifrost.Core.Domain.Document;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    /// <summary>
    /// Provides an interface for saving a document's state
    /// Used to see if a document has been modified 
    /// </summary>
    public interface IDocumentStateService
    {
        /// <summary>
        /// Loads the given document's persisted state
        /// If no state is found, null is returned
        /// </summary>
        /// <param name="category"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        DocumentState Get(string category, string documentId);

        /// <summary>
        /// Pushes all documents that should be verified to the Queue
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns> 
        int PushVerifyDocumentsToQueue(string category);
        /// <summary>
        /// Saves or updates the given documentstate
        /// </summary>
        /// <param name="category"></param>
        /// <param name="documentState"></param>
        void SaveOrUpdate(string category, DocumentState documentState);

        void Delete(string category, string documentId);

        /// <summary>
        /// Delete all items related to the given category
        /// </summary>
        /// <param name="category"></param>
        void Delete(string category);
    }

}