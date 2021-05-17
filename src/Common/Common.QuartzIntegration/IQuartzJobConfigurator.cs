using System.Collections.Generic;
using Bifrost.Common.QuartzIntegration.Domain;

namespace Bifrost.Common.QuartzIntegration
{
    public interface IQuartzJobConfigurator
    {
        /// <summary>
        ///     Retrieves all jobs from the quartz job storage
        /// </summary>
        /// <returns></returns>
        IList<TypedJob> GetJobs();

        /// <summary>
        ///     Saves the given job
        ///     If the job exists (same Name), updates the existing one
        /// </summary>
        /// <param name="job"></param>
        void SaveOrUpdateJob(IList<TypedJob> job);
    }
}