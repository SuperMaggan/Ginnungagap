using Bifrost.Dto.Apis;

namespace Bifrost.RestClient
{
    public interface IAdminClient
    {
        IJobApi Job { get; }

        IConsumerApi Consumer { get; }

    }
}