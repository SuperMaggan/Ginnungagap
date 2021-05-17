using System.Collections.Generic;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;
using Serilog;

namespace Bifrost.Core.ApplicationServices.Implementations
{
    public class NullIntegrationService : IIntegrationService
    {
        

        public bool CanConnect()
        {
            Log.Information("NullIntegrationService: CanConnect() => true");
            return true;
        }

        public void HandleDocuments(IList<IDocument> documents)
        {
            Log.Information($"NullIntegrationService: HandleDocuments() received {documents.Count} documents");
        }

        public void AddDocuments(IList<AddDocument> documents,string domain)
        {
            Log.Information($"NullIntegrationService: AddDocuments() received {documents.Count} {domain} documents");
        }

        public void DeleteDocuments(IList<DeleteDocument> docs, string domain)
        {
            Log.Information($"NullIntegrationService: DeleteDocuments() received {docs.Count} {domain} ids to delete");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.GetType().Name}: CanConnect()";
        }
    }
}