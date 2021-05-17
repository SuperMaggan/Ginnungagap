using System.Linq;
using Bifrost.Core.Connectors.States;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.QuartzIntegration.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Serilog;

namespace Bifrost.QuartzIntegration.Jobs.Internal
{
    public class ResumePausedJobsJob : ConcurrentJobBase, IInternalJob
    {

        private readonly IJobService _jobService;
        private readonly IStateService _stateService;

        public ResumePausedJobsJob(IJobService jobService, IStateService stateService)
        {
            _jobService = jobService;
            _stateService = stateService;
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Execute();
        }

        public void Execute()
        {
            var jobStates = _stateService.LoadStates().ToArray();
            foreach (var state in jobStates)
            {
                var connectorState = new ConnectorStateBase(state);
                if (connectorState.State == JobState.Paused)
                {
                    Log.Information($"Resuming PAUSED job {state.Name} to its previous state {connectorState.LastWorkingState}");
                    connectorState.State = connectorState.LastWorkingState;
                    _stateService.SaveState(connectorState);
                }
            }
        }

        public void Schedule(IScheduler scheduler)
        {
            var jobDetail = new JobDetailImpl(
                "ResumePausedJobsJob",
                "SPECIAL",
                typeof(ResumePausedJobsJob)
                , true, false);
            var jobTrigger = new CronTriggerImpl(
                "ResumePausedJobsJob-Trigger",
                "SPECIAL",
                "0 0 * * * ?"
                );
            scheduler.ScheduleJob(jobDetail, jobTrigger);
        }
    }
}
