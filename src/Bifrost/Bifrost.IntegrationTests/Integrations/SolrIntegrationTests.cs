using System.Collections.Generic;
using Bifrost.Integration.Solr;
using Bifrost.Core.Domain.Document;
using Bifrost.Common.Core.Domain;
using FluentAssertions;
using Xunit;

namespace Bifrost.IntegrationTests.Integrations
{
 
    public class SolrIntegrationTests
    {


        [Fact]
        public void Can_add_document()
        {
            var service = new SolrIntegrationService(new SolrSettings());
            var document = new AddDocument("magnus", "test")
            {
                Fields = new List<Field>()
                {
                    new Field("Kite", "4life")
                }
            };
            service.CanConnect().Should().BeTrue();
            service.HandleDocuments(new List<IDocument>(){document});

        }

        [Fact]
        public void Can_add_and_delete_document()
        {
            var service = new SolrIntegrationService(new SolrSettings());
            var document = new AddDocument("Im_gone", "test")
               {
                   Fields = new List<Field>()
                   {
                       new Field("Kite", "4life")
                   }
               };
               service.CanConnect().Should().BeTrue();
               service.HandleDocuments(new List<IDocument>() { document });

            var delDoc = new DeleteDocument(document.Id, document.Domain);
            service.HandleDocuments(new List<IDocument>(){ delDoc });


        }

        [Fact]
        public void Can_add_and_patch_document()
        {
            var service = new SolrIntegrationService(new SolrSettings());
            var document = new AddDocument("Patch_Me", "test")
            {
                Fields = new List<Field>()
                {
                    new Field("document_id", "Patch_Me"),
                    new Field("title", "Patch catch"),
                    new Field("source", "green")
                }
            };

            service.CanConnect().Should().BeTrue();
            service.HandleDocuments(new List<IDocument>() { document });
            var patchDoc = new PatchDocument("Patch_Me","test")
            {
                Fields = new List<Field>()
                {
                    new Field("document_id", "Patch_Me"),
                    new Field("title", "4life"),
                    new Field("source", "red")
                }
            };

            service.HandleDocuments(new List<IDocument>{ patchDoc});

        }

        [Fact]
        public void Can_add_patch_document()
        {
            var service = new SolrIntegrationService(new SolrSettings());

            service.CanConnect().Should().BeTrue();
            var patchDoc = new PatchDocument("Add_Me","test")
            {
                Fields = new List<Field>()
                {
                    new Field("document_id", "Add_Me"),
                    new Field("title", "hippie")
                }
            };

            service.HandleDocuments(new List<IDocument>{ patchDoc});

        }
    }
}
