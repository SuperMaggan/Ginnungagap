using System;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Quartz;

namespace Bifrost.Common.QuartzIntegration.Jobs
{
    /// <summary>
    /// Base class for a job that is configurable using IJobConfiguration
    /// </summary>
    /// <typeparam name="T">Implementation of IJobConfiguration that this job can handle</typeparam>
    public abstract class ConfigurableJobBase<T> : ConcurrentJobBase, IConfigurableJob
        where T: class, IJobConfiguration
    {
        protected readonly IJobService JobService;

        
        protected ConfigurableJobBase( IJobService jobService)
        {
            JobService = jobService;
        }

        /// <summary>
        /// Get the nam of th erunning job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected string GetJobName(IJobExecutionContext context)
        {
            return context.JobDetail.Key.Name;
        }

        /// <summary>
        /// Can this Job handle the given configuration
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        public bool CanHandle(IJobConfiguration jobConfiguration)
        {
            return jobConfiguration is T;
        }

        protected T GetConfiguration(string jobName)
        {
            var job = JobService.Get(jobName);
            if (job == null)
                throw new Exception($"No job named {jobName} could be found");
            var config = job.Configuration as T;
            config.JobName = jobName;
            return config;
        }
    }
}