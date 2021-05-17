using System;
using Bifrost.Common.Core.ApplicationServices;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Jobs.Internal
{
    public class UpdateProcessInformationJob : ConcurrentJobBase, IInternalJob
    {
        private readonly IProcessInformationService _processInformationService;

        public UpdateProcessInformationJob(IProcessInformationService processInformationService)
        {
            _processInformationService = processInformationService;
        }

        public void Execute()
        {
            try
            {
                _processInformationService.SaveCurrent("JobsRunner");
                Log.Verbose("Updated process information");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error when updating process information job: " +e.Message);
            }
        }

        public void Schedule(IScheduler scheduler)
        {
            var snapShot = _processInformationService.CreateSnapshot("JobsRunner");
            var lastPersistedProcInfo = _processInformationService.Get(snapShot.Id);

            var jobDetail = new JobDetailImpl(
                "UpdateProcessInformationJob",
                "SPECIAL",
                typeof (UpdateProcessInformationJob)
                , true, false);
            var jobTrigger = new SimpleTriggerImpl(
                "UpdateProcessInformationJob-Trigger",
                -1,
                new TimeSpan(0, 0,
                    lastPersistedProcInfo?.UpdateFrequencySec ?? snapShot.UpdateFrequencySec)
                );

            scheduler.ScheduleJob(jobDetail, jobTrigger);
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Execute();
        }
    }
}