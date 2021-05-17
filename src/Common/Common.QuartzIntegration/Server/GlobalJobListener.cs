using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Server
{
    public class GlobalJobListener : IJobListener
    {
        public GlobalJobListener()
        {
            Name = GetType().Name;
            Log.Debug("Initiated globaljoblistener");
        }

        Task IJobListener.JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Log.Verbose($"{context.JobDetail.Key.Name}.JobToBeExecuted");
            return Task.CompletedTask;
        }

        Task IJobListener.JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            Log.Verbose($"{context.JobDetail.Key.Name}.JobExecutionVetoed");
            return Task.CompletedTask;
        }

        Task IJobListener.JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = new CancellationToken())
        {
            Log.Verbose($"{context.JobDetail.Key.Name}.JobWasExecuted");
            if (jobException != null)
                Log.Error(jobException, $"{context.JobDetail.Key.Name} threw an exception");
            GC.Collect();
            return Task.CompletedTask;
        }

        public string Name { get; set; }

    }
}
