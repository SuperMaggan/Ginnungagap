using System.Collections.Generic;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.Core.ApplicationServices
{
    /// <summary>
    ///     Provides an interface for persisting jobs (eq. Quartz jobs)
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        ///     Retrieves all jobs
        ///     A job contains information about scheduling and execution way
        /// </summary>
        /// <returns></returns>
        IList<Job> Get();

        Job Get(string jobName);
        void Delete(string jobName);

        /// <summary>
        ///     Saves the given job
        ///     If the job exists (same Name), updates the existing one
        /// </summary>
        /// <param name="job"></param>
        void SaveOrUpdate(Job job);
    }
}