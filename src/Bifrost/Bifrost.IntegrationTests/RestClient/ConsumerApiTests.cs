using System.Collections.Generic;
using System.Linq;
using Bifrost.Dto.Dto;
using Bifrost.RestClient.Api;
using FluentAssertions;
using Xunit;

namespace Bifrost.IntegrationTests.RestClient
{
 
    public class ConsumerApiTests
    {


        [Fact]
        public void Can_add_and_load_consumer()
        {
            var client = new RestConsumerApi(new AutoResolveScyllaRestClientSettings());
            
            var consumer = new ConsumerDto()
            {
                Name = "IntegrationTestConsumer",
                CanEdit = true,
                IsAdmin = false,
                ApiKey = "1337",
                AvailableJobNames = new List<string>()
            };
            client.EditConsumer(consumer);

            var fetchedConsumer = client.GetConsumers(consumer.Name);
            fetchedConsumer.First().Should().Be(consumer);
        }

        [Fact]
        public void Can_add_and_load_and_delete_consumer()
        {
            var client = new RestConsumerApi(new AutoResolveScyllaRestClientSettings());

            var consumer = new ConsumerDto()
            {
                Name = "IntegrationTestConsumerToBeDeleted",
                CanEdit = true,
                IsAdmin = false,
                ApiKey = "asde",
                AvailableJobNames = new List<string>()
                {
                    "hej"
                }
            };
            client.EditConsumer(consumer);

            var fetchedConsumer = client.GetConsumers(consumer.Name);
            fetchedConsumer.First().Should().Be(consumer);

            client.DeleteConsumer(consumer.Name);

            fetchedConsumer = client.GetConsumers(consumer.Name);
            fetchedConsumer.Should().BeEmpty();


        }
    }
}
