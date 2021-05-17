using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Jobs
{
    public abstract class ConcurrentJobBase : IJob
    {
        private const string ConcurrentJobLimitKey = "ConcurrentJobLimit";

        private static readonly ConcurrentDictionary<string, Counter> ConcurrentJobsRunning =
            new ConcurrentDictionary<string, Counter>();
        

        public int GetConcurrentJobsRunning(string jobName)
        {
            return ConcurrentJobsRunning.GetOrAdd(jobName, new Counter()).Value;
        }

        public void IncrementConcurrentJobsRunning(string jobName)
        {
            ConcurrentJobsRunning.GetOrAdd(jobName, new Counter()).Increment();
        }

        public void DecrementConcurrentJobsRunning(string jobName)
        {
            ConcurrentJobsRunning.GetOrAdd(jobName, new Counter()).Decrement();
        }

        protected string[] GetArrayFromCsvEntry(IJobExecutionContext context, string fieldName)
        {
            var entry = context.JobDetail.JobDataMap.GetString(fieldName);
            if (string.IsNullOrEmpty(entry))
                return new string[0];
            return entry.Split(',', ';');
        }

        protected abstract void InternalExecute(IJobExecutionContext context);

        private static int GetConcurrentJobLimitFromContext(IJobExecutionContext context)
        {
            if (context.JobDetail.JobDataMap.Contains(ConcurrentJobLimitKey))
                return context.JobDetail.JobDataMap.GetIntValue(ConcurrentJobLimitKey);
            return 0;
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            var concurrentJobLimit = GetConcurrentJobLimitFromContext(context);
            long jobsRunning = GetConcurrentJobsRunning(context.JobDetail.Key.Name);

            if (concurrentJobLimit == -1)
                return Task.CompletedTask;

            if (concurrentJobLimit > 0)
            {
                if (jobsRunning >= concurrentJobLimit)
                {
                    Log.Debug(
                        $"Currently running {jobsRunning} jobs of type {GetType().FullName}. Can't run more since the ConcurrentJobLimit is {concurrentJobLimit}");
                    return Task.CompletedTask;
                }
            }

            IncrementConcurrentJobsRunning(context.JobDetail.Key.Name);
            jobsRunning++;
            Log.Debug($"Currently running {jobsRunning}/{concurrentJobLimit} jobs of type {GetType().FullName}");
            try
            {
                InternalExecute(context);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error when executing job {context.JobDetail.Key.Name} : " +e.Message );
            }
            finally
            {
                DecrementConcurrentJobsRunning(context.JobDetail.Key.Name);
            }
            return Task.CompletedTask;
        }
    }
}