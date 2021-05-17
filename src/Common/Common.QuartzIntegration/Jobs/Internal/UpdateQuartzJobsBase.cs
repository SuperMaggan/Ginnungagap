using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Jobs.Internal
{
    public abstract class UpdateQuartzJobsBase : ConcurrentJobBase
    {

        protected static DateTime RecentJobUpdate;
        protected static int RecentJobCount;

        protected const string ConfigurableJobsGroup = "CONFIGURABLEJOBS";
        protected readonly IJobService JobService;
        protected readonly IList<IConfigurableJob> ConfigurableJobTypes;

        protected UpdateQuartzJobsBase(IConfigurableJob[] configurableJobTypes, IJobService jobService)
        {
            ConfigurableJobTypes = configurableJobTypes;
            JobService = jobService;
        }

        protected bool AnyJobIsUpdated(IList<Job> jobs)
        {
            var isUpdated = false;

            if (!jobs.Any() && RecentJobCount > 0)
                return true;

            if (RecentJobCount != jobs.Count)
                isUpdated = true;
            else if (jobs.Any() && RecentJobUpdate <= jobs.Max(x => x.LastUpdated))
                isUpdated = true;

            RecentJobUpdate = DateTime.UtcNow;
            RecentJobCount = jobs.Count;
            return isUpdated;
        }

        protected Type GetConfigurableJobType(Job job)
        {
            if (ConfigurableJobTypes == null || !ConfigurableJobTypes.Any())
            {
                Log.Error(
                    $"Can't resolve the type of configuration for the given job ({job.Name}) since no IConfigurableJob implementations are registered");
                return null;
            }
            var configurableJob = ConfigurableJobTypes.FirstOrDefault(j => j.CanHandle(job.Configuration));
            if (configurableJob == null)
            {
                Log.Warning(
                    $"No IConfigurableJob registered that can handle jobType {job.Configuration.GetType().Name}. Make sure you've registered it in the container (it might just recently've been deleted)");

                return null;
            }
            return configurableJob.GetType();
        }
    }
}