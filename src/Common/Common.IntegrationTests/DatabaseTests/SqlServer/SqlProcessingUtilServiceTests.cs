using System;
using Bifrost.Common.ApplicationServices.Sql;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.Core.Domain.DocumentProcessing;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.IntegrationTests.DatabaseTests.SqlServer
{
    public class SqlProcessingUtilServiceTests
    {

        SqlProcessingUtilService CreateService()
        {
            return new SqlProcessingUtilService(new ProcessingUtilDatabase(new CommonDatabaseSettings()));
        }

        [Fact]
        public void CanSaveAndLoadStringObject()
        {
            var service = CreateService();

            var utilObject = new KeyValueUtilObject
            {
                Key = "test",
                Value =
                    "hej jag heter magnus och är väldigt glad. Om 3 veckor går jag på föräldraledighet. OMVÄRLDEN, hör mig! Nu gör Jag dock om testet 2 år senare. Vad visste egentligen jag som enbarnsförälder??"
            };


            service.SaveUtilObject(utilObject);

            var fetchedObject = service.GetUtilObject<KeyValueUtilObject>(utilObject.Key);

            fetchedObject.Should().NotBeNull();
            fetchedObject.Key.Should().Be(utilObject.Key);
            fetchedObject.Value.Should().Be(utilObject.Value);
        }

        [Fact]
        public void CanSaveAndLoadDocumentRelationObject()
        {
            var service = CreateService();

            var utilObject = new DocumentRelationUtilObject {Key = "test"};

            utilObject.DocumentIds.Add("första amdra");
            utilObject.DocumentIds.Add("I like me neighbours");

            service.SaveUtilObject(utilObject);

            var fetchedObject = service.GetUtilObject<DocumentRelationUtilObject>(utilObject.Key);

            fetchedObject.Should().NotBeNull();
            fetchedObject.Key.Should().Be(utilObject.Key);
            fetchedObject.DocumentIds.Should().BeEquivalentTo(utilObject.DocumentIds);
        }

        [Fact]
        public void CanDeleteUtilObject()
        {
            var service = CreateService();

            var utilObject = new DocumentRelationUtilObject {Key = Guid.NewGuid().ToString()};

            utilObject.DocumentIds.Add("första amdra");
            utilObject.DocumentIds.Add("I like me neighbours");

            service.SaveUtilObject(utilObject);

            var fetchedObject = service.GetUtilObject<DocumentRelationUtilObject>(utilObject.Key);

            fetchedObject.Should().NotBeNull();
            fetchedObject.Key.Should().Be(utilObject.Key);
            fetchedObject.DocumentIds.Should().BeEquivalentTo(utilObject.DocumentIds);

            service.DeleteUtilObject(utilObject.Key);

            fetchedObject = service.GetUtilObject<DocumentRelationUtilObject>(utilObject.Key);
            fetchedObject.Should().BeNull();
        }
    }
}