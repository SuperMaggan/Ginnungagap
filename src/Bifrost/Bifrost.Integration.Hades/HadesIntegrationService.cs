using System;
using System.Collections.Generic;
using System.Linq;
using Asgard.Asgard.Core.ApplicationServices.Interfaces;
using Asgard.Asgard.Core.Domain.Document;
using Asgard.Common.Core.Domain;
using Asgard.Hades.RestClient;
using Asgard.Hades.RestClient.Internal;

namespace Asgard.Integration.Hades
{
    public class HadesIntegrationService : IIntegrationService
    {
        private readonly IConsumerRestClient _client;

        public HadesIntegrationService(IHadesRestClientSettings settings)
        {
            if (settings.HadesTimeoutMS == 0)
                settings.HadesTimeoutMS = 10000;
            _client = new ConsumerRestClient(settings);
        }

        public bool CanConnect()
        {
            return true;
            
        }

        public void HandleDocuments(IList<IDocument> documents)
        {
            string domain = documents.FirstOrDefault()?.Domain;
            if (documents.Select(x => x.Domain).Distinct().Count() > 1)
                throw new NotSupportedException("No support for calling HandleDocuments() with documents of different domains.");
            AddDocuments(documents.Where(d => d is AddDocument).Cast<AddDocument>());
            PatchDocuments(documents.Where(d => d is PatchDocument).Cast<PatchDocument>());
            DeleteDocuments(documents.Where(d => d is DeleteDocument).Cast<DeleteDocument>());
        }

        private void DeleteDocuments(IEnumerable<DeleteDocument> deleteDocuments)
        {

            _client.Document.DeleteDocuments(deleteDocuments.Select(x => x.ToDto()).ToList());
        }

        private void PatchDocuments(IEnumerable<PatchDocument> patchDocuments)
        {
            foreach (var patchDocument in patchDocuments)
            {
                _client.Document.PatchDocument(patchDocument.ToDto());
            }
        }

        private void AddDocuments(IEnumerable<AddDocument> addDocuments)
        {
            _client.Document.AddDocuments(addDocuments.Select(x=>x.ToDto()).ToList());
        }


        
    }
}
