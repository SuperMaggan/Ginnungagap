using System.Collections.Generic;
using Bifrost.Dto.Apis;
using Bifrost.Dto.Dto;
using Bifrost.RestClient.Internal;

namespace Bifrost.RestClient.Api
{
    public class RestConsumerApi : RestClientBase, IConsumerApi
    {
        private readonly string _url;

        public RestConsumerApi(IAsgardRestClientSettings settings) : base(settings)
        {
            _url = $"{settings.AsgardApiUrl.Trim('/')}/api/v1";
        }


        public IList<ConsumerDto> GetConsumers(params string[] consumerNames)
        {
            return Get<List<ConsumerDto>>($"{_url}/consumer?{QueryParameter("consumerNames", consumerNames)}");
        }


        public void EditConsumer(ConsumerDto consumer)
        {
            Put($"{_url}/consumer", consumer);
        }

        public void DeleteConsumer(string consumerName)
        {
            Delete($"{_url}/consumer?consumerName={consumerName}");
        }

    }
}
