using System.Collections.Generic;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Core.ApplicationServices.Interfaces
{
    /// <summary>
    /// Provides an interface for communicating with an integration point
    /// </summary>
    public interface IIntegrationService
    {
        /// <summary>
        /// Tries to ping the instance to verify if its up and responding
        /// </summary>
        /// <returns></returns>
        bool CanConnect();

        /// <summary>
        /// Sends a set of documents for adding or deleting
        /// </summary>
        /// <param name="documents"></param>
        void HandleDocuments(IList<IDocument> documents);

    }
}
