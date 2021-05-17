using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bifrost.Common.QuartzIntegration.Jobs;
using Quartz;
using Quartz.Spi;
using Serilog;

namespace Bifrost.Common.QuartzIntegration.Server
{
    /// <summary>
    ///     The main server logic.
    /// </summary>
    public class QuartzServer : IQuartzServer
    {
        private readonly IList<IInternalJob> _internalJobs;
        private readonly IJobFactory _jobFactory;
        private readonly IJobListener _jobListener;
        private IScheduler _scheduler;
        private readonly ISchedulerFactory _schedulerFactory;

        /// <summary>
        ///     INitializes the QuartzServer without any internal jobs
        /// </summary>
        /// <param name="jobFactory"></param>
        /// <param name="jobListener"></param>
        /// <param name="factory"></param>
        public QuartzServer(IJobFactory jobFactory, IJobListener jobListener, ISchedulerFactory factory)
            : this(jobFactory, jobListener, factory, new IInternalJob[0])
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuartzServer" /> class. The given internal jobs will be executed once
        ///     and
        ///     then scheduled as normal jobs
        /// </summary>
        public QuartzServer(IJobFactory jobFactory, IJobListener jobListener, ISchedulerFactory factory, IInternalJob[] internalJobs)
        {
            _jobFactory = jobFactory;
            _jobListener = jobListener;
            _internalJobs = internalJobs;
            _schedulerFactory = factory;
        }


        /// <summary>
        ///     Returns the current scheduler instance (usually created in <see cref="Initialize" />
        ///     using the <see cref="GetScheduler" /> method).
        /// </summary>
        protected virtual IScheduler Scheduler => _scheduler;

        /// <summary>
        ///     Initializes the instance of the <see cref="QuartzServer" /> class.
        /// </summary>
        public virtual void Initialize()
        {
            try
            {
                foreach (var internalJob in _internalJobs)
                {
                    internalJob.Execute();
                }

                _scheduler = GetScheduler();
                _scheduler.JobFactory = _jobFactory;
                _scheduler.ListenerManager.AddJobListener(_jobListener);
                ScheduleInternalJobs();
            }
            catch (Exception e)
            {
                Log.Error(e,"Server initialization failed:" + e.Message);
                throw;
            }
        }

        /// <summary>
        ///     Starts this instance, delegates to scheduler.
        /// </summary>
        public virtual void Start()
        {
            _scheduler.Start();

            try
            {
                Thread.Sleep(3000);
            }
            catch (ThreadInterruptedException)
            {
            }

            Log.Information("Scheduler started successfully");
        }

        /// <summary>
        ///     Stops this instance, delegates to scheduler.
        /// </summary>
        public virtual void Stop()
        {
            _scheduler.Shutdown(true);
            Log.Information("Scheduler shutdown complete");
        }

        /// <summary>
        ///     Pauses all activity in scheudler.
        /// </summary>
        public virtual void Pause()
        {
            _scheduler.PauseAll();
        }

        /// <summary>
        ///     Resumes all acitivity in server.
        /// </summary>
        public void Resume()
        {
            _scheduler.ResumeAll();
        }

        private void ScheduleInternalJobs()
        {
            if (!_internalJobs.Any())
                return;

            foreach (var internalJob in _internalJobs)
            {
                Log.Information($"Scheduling internal quartz jobs: {internalJob.GetType().Name}");
                internalJob.Schedule(_scheduler);
            }
        }

        /// <summary>
        ///     Gets the scheduler with which this server should operate with.
        /// </summary>
        /// <returns></returns>
        protected virtual IScheduler GetScheduler()
        {
            return _schedulerFactory.GetScheduler().Result;
        }

       
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            // no-op for now
        }
    }
}