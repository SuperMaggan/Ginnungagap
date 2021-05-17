using Bifrost.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Core.Connectors
{
    public interface IConnector<TConfig>
        where TConfig : IJobConfiguration
    {
        /// <summary>
        /// Initiates a crawl. 
        /// Will fetch fetch a number of changes in the source (decided by batch size)
        /// </summary>
        /// <returns></returns>
        SourceChanges ExecuteFetch(TConfig config);

        /// <summary>
        /// Deletes any state, resetting the job
        /// </summary>
        void ResetConnector(string jobName);
    }
}