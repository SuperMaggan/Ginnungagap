using Bifrost.Common.Core.Domain.Jobs;
using Quartz;

namespace Bifrost.Common.QuartzIntegration.Jobs
{
    /// <summary>
    /// Represents a job that is configurable using IJobConfiguration
    /// </summary>
    public interface IConfigurableJob : IJob
    {
        /// <summary>
        /// Can this Job handle the given configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        bool CanHandle(IJobConfiguration jobConfiguration);
    }
}