using System;
using System.Linq;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.QuartzIntegration.Domain;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Jobs.Internal
{
    /// <summary>
    ///     Grabs all globally configured job found in the given implementation of IJobRepository and
    ///     serializes them using IQuartzJobConfigurator
    /// </summary>
    public class UpdateQuartzJobsFileJob : UpdateQuartzJobsBase, IInternalJob
    {

        private readonly IQuartzJobConfigurator _quartzJobConfigurator;

        public UpdateQuartzJobsFileJob(IConfigurableJob[] configurableJobTypes, IJobService jobService, IQuartzJobConfigurator quartzJobConfigurator) : base(configurableJobTypes, jobService)
        {
            _quartzJobConfigurator = quartzJobConfigurator;
        }

        public void Execute()
        {
            try
            {
                var globalJobs = JobService.Get();
                if (!AnyJobIsUpdated(globalJobs))
                    return;
                foreach (var globalJob in globalJobs)
                { 
                    if (!globalJob.Enabled)
                        globalJob.ConcurrentLimit = -1;
                }


                _quartzJobConfigurator.SaveOrUpdateJob(
                    globalJobs.Select(JobToTypedJob).ToList());
                Log.Verbose($"Update local quartz jobs file with {globalJobs.Count} global jobs");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error when updating the quartz jobs file: " +e.Message);
            }
        }

        public void Schedule(IScheduler scheduler)
        {
            var jobDetail = new JobDetailImpl(
                "UpdateLocalQuartzJobs",
                "SPECIAL",
                typeof (UpdateQuartzJobsFileJob)
                , true, false);
            var jobTrigger = new SimpleTriggerImpl(
                "UpdateLocalQuartzJobs-Trigger",
                -1,
                new TimeSpan(0, 0, 15)
                );
            scheduler.ScheduleJob(jobDetail, jobTrigger);
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Execute();
        }

        private TypedJob JobToTypedJob(Job job)
        {
            var jobType= GetConfigurableJobType(job);
            return new TypedJob()
            {
                Name = job.Name,
                Description = job.Description,
                ConcurrentLimit = job.ConcurrentLimit,
                Enabled = job.Enabled,
                JobType = jobType,
                TriggerCronSyntax = job.TriggerCronSyntax,
                Configuration = job.Configuration,
                LastUpdated = job.LastUpdated
            };
        }
    }
}