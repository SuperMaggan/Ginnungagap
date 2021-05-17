using System;
using System.Diagnostics;
using System.Text;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.Domain.Jobs;
using Quartz;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Jobs
{
    public abstract class ConfigurableJobWithStateBase<T> : ConfigurableJobBase<T> where T : class, IJobConfiguration
    {
        protected readonly IStateService StateService;
        protected ConfigurableJobWithStateBase(IJobService jobService, IStateService stateService) : base(jobService)
        {
            StateService = stateService;
        }

        protected override void InternalExecute(IJobExecutionContext context)
        {
            var jobName = GetJobName(context);
            var config = GetConfiguration(jobName);
            var state = GetJobState(jobName);
            var stateMessage = new StringBuilder();
            var timer = new Stopwatch(); timer.Start();
            state.LastExecutionDate = DateTime.UtcNow;
            try
            {
                DoWork(config, state, stateMessage);
            }
            catch (Exception e)
            {
                state.SetErrorState(e);
                Log.Error(e, $"Error when executing job, {this.GetType().Name}: {e.Message}");
            }
            finally
            {
                state.Message = stateMessage.ToString();
                state.LastExecutionTimeMs = (int)timer.ElapsedMilliseconds;
                StateService.SaveState(state);
            }
        }

        /// <summary>
        /// Do all work here
        /// Feel free to throw exceptions
        /// </summary>
        /// <param name="config"></param>
        /// <param name="jobState"></param>
        /// <param name="progressInformation"></param>
        protected abstract void DoWork(T config, JobState jobState, StringBuilder progressInformation);

        protected JobState GetJobState(string jobName)
        {
            var state = StateService.LoadState(jobName);

            return state == null ? new JobState() { Name = jobName } : new JobState(state);
        }
    }
}