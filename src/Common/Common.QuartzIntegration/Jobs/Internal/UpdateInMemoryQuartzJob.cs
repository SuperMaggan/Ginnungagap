using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.QuartzIntegration.Helpers;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Jobs.Internal
{
    /// <summary>
    ///     Grabs all globally configured job found in the given implementation of IJobRepository and
    ///     serializes them using IQuartzJobConfigurator
    /// </summary>
    public class UpdateInMemoryQuartzJob : UpdateQuartzJobsBase, IInternalJob
    {
        private readonly ISchedulerFactory _schedulerFactory;


        public UpdateInMemoryQuartzJob(IConfigurableJob[] configurableJobTypes, IJobService ijobService, ISchedulerFactory schedulerFactory) : base(configurableJobTypes, ijobService)
        {
            _schedulerFactory = schedulerFactory;
        }

        public void Execute()
        {
            try
            {
                var globalJobs = JobService.Get();
                if(!AnyJobIsUpdated(globalJobs))
                    return;
                var scheduler = _schedulerFactory.GetScheduler();

                var triggers = scheduler.Result.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(ConfigurableJobsGroup));
                scheduler.Result.UnscheduleJobs(triggers.Result.ToArray());


                foreach (var globalJob in globalJobs)
                { 
                    if (!globalJob.Enabled)
                        globalJob.ConcurrentLimit = -1;
                    ScheduleJob(globalJob,scheduler.Result);
                }

                
                Log.Verbose($"Updated in-memory jobs with {globalJobs.Count} global jobs");
            }
            catch (Exception e)
            {
                Log.Error(e,"Error when updating in-memory jobs:" + e.Message);
            }
        }

        private void ScheduleJob(Job job, IScheduler scheduler)
        {
            var configurableJobType = GetConfigurableJobType(job);
            if (configurableJobType == null) { 
                return;
            }
            var jobDetails =JobBuilder.Create(configurableJobType)
                .SetJobData(CreateDataMap(job))
                .WithDescription(job.Description)
                .StoreDurably(false)
                .WithIdentity(job.Name, ConfigurableJobsGroup)
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetails)
                .WithIdentity(job.TriggerName(), ConfigurableJobsGroup)
                .WithCronSchedule(job.TriggerCronSyntax)
                .Build();

            scheduler.ScheduleJob(jobDetails, trigger);
        }


        private JobDataMap CreateDataMap(Job job)
        {
            return  new JobDataMap {new KeyValuePair<string, object>("ConcurrentJobLimit", job.ConcurrentLimit)};
        }
      
        
        public void Schedule(IScheduler scheduler)
        {
            var jobDetail = new JobDetailImpl(
                "UpdateInMemoryQuartzJob",
                "SPECIAL",
                typeof (UpdateInMemoryQuartzJob)
                , true, false);
            var jobTrigger = new SimpleTriggerImpl(
                "UpdateInMemoryQuartzJob-Trigger",
                -1,
                new TimeSpan(0, 0, 15)
                );

             
            scheduler.DeleteJob(jobDetail.Key);
            scheduler.ScheduleJob(jobDetail, jobTrigger);
        }



        protected override void InternalExecute(IJobExecutionContext context)
        {
            Execute();
        }


    }
}