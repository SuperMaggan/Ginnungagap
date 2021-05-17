using Bifrost.Dto.Apis;
using Bifrost.RestClient.Api;
using Bifrost.RestClient.Internal;

namespace Bifrost.RestClient
{
    public class RestAdminClient : IAdminClient
    {
        public RestAdminClient(IAsgardRestClientSettings settings)
        {
            Job = new RestJobApi(settings);
            Consumer = new RestConsumerApi(settings);
        }

        public IJobApi Job { get; }
        public IConsumerApi Consumer { get; }
    }
}